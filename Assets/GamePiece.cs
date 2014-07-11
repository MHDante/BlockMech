using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
[ExecuteInEditMode]
public enum PieceType
{
    none,
    antitrap,
    button,
    end,
    key,
    keyhole,
    player,
    start,
    teleport,
    trap,
    wall,
}
public abstract class GamePiece : MonoBehaviour
{
    public Cell cell;
    public PieceType piecetype;
	private const int defaultWeight  = 1;
	Cell destination;
	bool isMoving = false;
	public GamePiece container{get{
			GamePiece ret = cell.gamePiece;
			if (ret == this) return null;
			while (ret.containedPiece != this){
				if (ret == null) throw new WTFException();
				ret = ret.containedPiece;
			} return ret;

		}}
    public abstract bool isSolid { get; set; }

	private int _weight = defaultWeight;
	public virtual int weight { get{return _weight;} set{_weight=value;}}

    public abstract bool isPushable { get; set; }

    public GamePiece containedPiece { get ;set; }

    public virtual bool pushFrom(Side side, int strength = 1)
    {
		if(!isPushable) return false;
		Wall w = cell.getWall(Utils.opposite(side));
		if(w!=null && !w.isTraversible) return false;
		if (strength < weight) return false;
		GamePiece obstructor = cell.getNeighbour(side).gamePiece;

		if(obstructor!=null)return moveTo(Utils.opposite(side));
		if(obstructor.isSolid && !isPushable) return false;
		if(!obstructor.isSolid && !isPushable) return moveTo(Utils.opposite(side));
		bool obsPushed = obstructor.pushFrom(side, strength - weight);
		if (obsPushed){
			bool succeed = moveTo(Utils.opposite(side));
			if (!succeed){obstructor.moveTo(side);}
			return succeed;
		} else return false;
    }
    public virtual bool onOccupy(GamePiece piece)
    {
		if (containedPiece = null){
        	containedPiece = piece;
			return true;
		} return false;
    }

    public virtual GamePiece onDeOccupy() 
    {
        GamePiece temp = containedPiece;
        containedPiece = null;
        return temp;
    }

    public virtual bool moveTo(Side side) {
		if (isMoving) return false;
		Cell dest = cell.getNeighbour(side);
        if (dest == null) return false;
        bool available = dest.Reserve();//Intentional Set.
		if (available)
        {
            isMoving = true;
            destination = dest;
        }
        return available;
	}
	public virtual bool TeleportTo(Cell target){
		Cell currentCell = cell;
		if (target.IsSolidlyOccupied())return false;
		DeOccupyParent();
		if (!target.Occupy(this)) {
			currentCell.Occupy(this);
			return false;
		} return true;

	}
	void DeOccupyParent(){
		if (container == null){
			if (this != cell.DeOccupy()) throw new WTFException();
		} else {
			container.onDeOccupy();
		}
	}


    internal void Destroy(bool destroyChildren = false)
    {
        if (destroyChildren)
        {
            if (container == null)
            {
                if (cell.DeOccupy() != this) throw new WTFException();
            }
            else
            {
                container.containedPiece = null;
            }
            DestroyImmediate(this.gameObject);
            if (containedPiece != null) containedPiece.Destroy(destroyChildren);
        }
        else
        {
            if (containedPiece == null)
            {
                this.Destroy(true);
                return;
            }
            else if (container == null)
            {
                cell.gamePiece = containedPiece;
            }
            else
            {
                container.containedPiece = containedPiece;
            }
            DestroyImmediate(this.gameObject);
        }



    }
    public float speed = 5f;
    public float currentLerp = 0f, maxLerp = 100f;
	public virtual void Update()
    {
        if (isMoving)
        {
            if (currentLerp >= maxLerp)
            {
                currentLerp = 0f;
                isMoving = false;
                transform.position = destination.WorldPos();
                destination.Unreserve();
                cell = destination;
                destination = null;
            }
            else
            {
                transform.position = Vector2.Lerp(cell.WorldPos(), destination.WorldPos(), currentLerp / 100f);
                currentLerp += speed;
            }
        }
	}
	public virtual void Awake(){}
	public virtual void Start(){}
}


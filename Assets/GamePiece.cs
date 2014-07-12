using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
public enum PieceType
{
    none,
    wall,
    door,
    player,
    end,
    button,
    switcH,
    key,
    keyhole,
    teleport,
    tile,
    trap,
    antitrap,
}
[ExecuteInEditMode]
public abstract class GamePiece : MonoBehaviour
{
    public Cell _cell ;
    public Cell cell { get { return _cell; } set { _cell = value; if (value!=null)transform.position = value.WorldPos(); } }
    public PieceType piecetype;
	private const int defaultWeight  = 1;
	Cell destination;
	public bool isMoving = false;
	public GamePiece container { get
    {
        
			
            if (cell == null) 
                return null;
			GamePiece ret = cell.gamePiece;
			if (ret == this || ret ==null) return null;

			while (ret.containedPiece != this) {
				if (ret == null) throw new WTFException();
				ret = ret.containedPiece;
                if (ret == null)
                    Debug.Log("ErrorEngine");
			} return ret;

		}
    }
    public abstract bool isSolid { get; set; }

	private int _weight = defaultWeight;
	public virtual int weight { get{return _weight;} set{_weight=value;}}

    public abstract bool isPushable { get; set; }

    public GamePiece containedPiece { get ;set; }

    public virtual void Awake() {
        
    }
    public virtual void Start() 
    {
        if (!Cell.GetFromWorldPos(transform.position).Occupy(this)) 
            throw new WTFException(this.GetType().ToString());
    }

    public virtual bool pushFrom(Side side, int strength = 1)
    {
		if(!isSolid && this.containedPiece!= null && this.containedPiece.pushFrom(side, strength))
			return true;
		if(!isPushable) 
			return false;
		Wall w = cell.getWall(Utils.opposite(side));
		if(w!=null && !w.isTraversible) 
			return false;
		if (strength < weight) 
			return false;
		GamePiece obstructor = GetNeighbour(Utils.opposite(side));

		if(obstructor==null)return moveTo(Utils.opposite(side));
		if(obstructor.isSolid && !isPushable) 
			return false;
		if(!obstructor.isSolid && !isPushable) 
			return moveTo(Utils.opposite(side));
		bool obsPushed = obstructor.pushFrom(side, strength - weight);
		if (obsPushed){
			obstructor.Detatch();
			bool succeed = moveTo(Utils.opposite(side));
			if (!succeed){obstructor.moveTo(side);}
			return succeed;
		} 
		return false;
    }
    public virtual bool onOccupy(GamePiece piece)
    {
		if (containedPiece == null){//not intentional set
        	containedPiece = piece;
			return true;
        } return containedPiece.onOccupy(piece);
    }

    public virtual GamePiece onDeOccupy() 
    {
        GamePiece temp = containedPiece;
        containedPiece = null;
        return temp;
    }
	public GamePiece GetNeighbour(Side s){
		Cell neighbour = cell.getNeighbour(s); 
		if(neighbour == null) return null;
        return neighbour.gamePiece;
	}
    public virtual bool moveTo(Side side) {
		if (isMoving) return false;
        if (cell == null)
            return false;
		Cell dest = cell.getNeighbour(side);
        Wall w = cell.getWall(side);
        if(w!=null && !w.isTraversible) return false;
        if (dest == null) return false;
        bool available = dest.Reserve();//Intentional Set.
		if (available)
        {
            isMoving = true;
            StartPos = cell.WorldPos();
            destination = dest;
        }
        return available;
	}
	public virtual bool TeleportTo(Cell target){
		Cell currentCell = cell;
		if (target.IsSolidlyOccupied())return false;
		Detatch();
		if (!target.Occupy(this)) {
			currentCell.Occupy(this);
			return false;
		} return true;

	}

    public void Detatch(bool bringChildren = true)
    {
        if (bringChildren)
        {
            if (cell == null) return;
            if (container == null)
            {
                if (cell.Empty() != this) throw new WTFException();
            }
            else
            {
                container.onDeOccupy();
            }
        }
        else
        {
            if (containedPiece == null)
            {
                this.Detatch(true);
                return;
            }
            else if (cell == null) 
            {
                return;
            }
            else if (container == null)
            {
                cell.gamePiece = containedPiece;
            }
            else
            {
                container.containedPiece = containedPiece;
                if (containedPiece == null) container.onDeOccupy();
                this.containedPiece = null;
            }
        }
    }
    public void Destroy(bool destroyChildren = true)
    {
        Detatch(destroyChildren);
        GamePiece g = this;
        while (g!=null){
            if (this.gameObject)DestroyImmediate(this.gameObject);
            g = g.containedPiece; 
        }
        
    }
    public float speed = 5f;
    public float currentLerp = 0f, maxLerp = 100f;
    public Vector2 StartPos;
	public virtual void Update()
    {
        if (isMoving)
        {
            if (currentLerp >= maxLerp)
            {
                currentLerp = 0f;
                isMoving = false;
                Detatch();

                
                destination.Unreserve();
                destination.Occupy(this);
                destination = null;
            }
            else
            {
                transform.position = Vector2.Lerp(StartPos, destination.WorldPos(), currentLerp / 100f);
                currentLerp += speed;
            }
        }
	}
    public virtual void OnDestroy(){
        if(cell != null)Detatch();
    }
}


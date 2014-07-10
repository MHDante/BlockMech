using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;


public abstract class GamePiece : MonoBehaviour
{
	private const int defaultWeight  = 1;

    public Cell cell;
	Cell target;
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

    public GamePiece containedPiece { get ;set;}

    public virtual bool pushFrom(Side side, int strength = 1)
    {
		if(!isPushable) return false;
		Wall w = cell.GetWall(Utils.opposite(side));
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
		target = cell.getNeighbour(side);
		return isMoving = target.Reserve();//Intentional Set.

	}
	public virtual bool TeleportTo(Cell target){
		if (target.IsSolidlyOccupied())return false;
		//if DeOccupy();
		bool canMove = target.Occupy(this);
		return canMove;

	}
	void DeOccupy(){
		if (container == null){
			if (this != cell.DeOccupy()) throw new WTFException();
		} else {
			container.onDeOccupy();
		}
	}

}


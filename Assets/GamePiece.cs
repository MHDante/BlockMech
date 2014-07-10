using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;


public abstract class GamePiece : MonoBehaviour
{
    public Cell cell;

    public enum State
    {
        moving,
		idle,
		disabled
    }
	public GamePiece container{get{
			GamePiece ret = cell.gamePiece;
			if (ret == this) return null;
			while (ret.containedPiece != this){
				if (ret == null) throw new WTFException();
				ret = ret.containedPiece;
			} return ret;

		}}
    public abstract bool isSolid { get; set; }

    public abstract bool isPushable { get; set; }

    public GamePiece containedPiece { get ;set;}

    public virtual bool pushFrom(Side side)
    {
		if (!isPushable) return false;
		moveTo(Utils.opposite(side));
        return true;
    }
    public virtual bool onOccupy(GamePiece piece)
    {
        containedPiece = piece;
        return true;
    }

    public virtual GamePiece onDeOccupy() 
    {
        GamePiece temp = containedPiece;
        containedPiece = null;
        return temp;
    }

    public virtual bool moveTo(Side side) {
		Cell target = cell.getNeighbour(side);
		return moveTo(target.x,target.y);  
	}
	public virtual bool moveTo(int x, int y){
		DeOccupy();
		RoomManager.roomManager.Grid[x][y].Occupy(this);
		return true;
	}
	void DeOccupy(){
		if (container == null){
			cell.DeOccupy();
		} else {
			container.onDeOccupy();
		}
	}

}


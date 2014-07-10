using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

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

    public GamePiece containedPiece { get ;set; }

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
}


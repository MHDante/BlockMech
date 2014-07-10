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

    public abstract bool isSolid { get; set; }

    public abstract bool isPushable { get; set; }

    public GamePiece containedPiece { get ;set;}

    public virtual bool pushFrom(Side side)
    {
		if (!isPushable) return false;
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

    public virtual bool move(Side side) {
		return true;  
	}
}


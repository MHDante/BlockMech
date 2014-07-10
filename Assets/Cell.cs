using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public enum Side { top, right, bottom, left };
public class Cell {

    public int x { get; set; }
    public int y { get; set; }
	RoomManager room;

    private Dictionary<Side, Wall> walls = new Dictionary<Side, Wall>();

	public GamePiece gamePiece{ get; set;}

    public List<GamePiece> getPiecesOnCell()
    {
        List<GamePiece> list = new List<GamePiece>();
        if (gamePiece != null)
        {
            getPiecesOnCellRecurse(list, gamePiece);
        }
        return list;
    }
    private void getPiecesOnCellRecurse(List<GamePiece> list, GamePiece current)
    {
        if (current == null) return;
        list.Add(current);
        getPiecesOnCellRecurse(list, current.containedPiece);
    }

	public bool Occupy(GamePiece piece){
		if(gamePiece == null){
            piece.cell = this;
			gamePiece = piece;
			return true;
		} 
		GamePiece g = gamePiece;
		while(g.containedPiece!=null){
			if(g.isSolid)return false;
			g=g.containedPiece;
		}
		if (g.onOccupy(piece))
        {
            piece.cell = this;
        }
		return true;
	}
	public GamePiece DeOccupy(){
		GamePiece ret = gamePiece;
		gamePiece = null;
        return ret;
	}

	public Cell getNeighbour(Side s){
		try{
			switch(s){
			case Side.bottom:
				return room.Grid[x][y-1];
			case Side.top:
				return room.Grid[x][y+1];
			case Side.left:
				return room.Grid[x-1][y];
			case Side.right:
				return room.Grid[x+1][y];
            } throw new Exception("WHAT THE FUCK");
		} catch (IndexOutOfRangeException e){
			return null;
		}
	}

	public Wall getWall(Side s){
        if (walls.ContainsKey(s))
		    return walls[s];
        return null;
	}
    public bool setWall(Side s, Wall w)
    {
        walls[s] = w;
        return true;
    }

	public bool isOccupied(){
		if (gamePiece == null) return false;
		GamePiece g = gamePiece.containedPiece;
		while (g!= null){
		if (g.isSolid) return true;
			g=g.containedPiece;
		} return false;
	}

    public Cell(RoomManager room, int x, int y)
    {
        this.room = room;
        this.x = x;
        this.y = y;
        walls = new Dictionary<Side, Wall>();
        foreach(Side s in Enum.GetValues(typeof(Side)))
        {
            walls[s] = null;
        }
    }
}

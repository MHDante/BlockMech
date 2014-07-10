using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public enum Side { top, right, bottom, left };
public class Cell {

    public int x { get; set; }
    public int y { get; set; }
	RoomManager room;

	public static Cell Get(int x, int y){return RoomManager.roomManager.Grid[x][y];}

    public Dictionary<Side, Wall> walls = new Dictionary<Side, Wall>();

	public GamePiece gamePiece{ get; private set;}

	public bool Occupy(GamePiece piece){
		if(gamePiece == null){
			gamePiece = piece;
			piece.cell = this;
			return true;
		} 
		GamePiece g = gamePiece.containedPiece;
		while(g!=null){
			if(g.isSolid)return false;
			g=g.containedPiece;
		}
		piece.cell = this;
		g.onOccupy(piece);

		return true;
	}
	public GamePiece DeOccupy(){
		GamePiece ret = gamePiece;
		gamePiece = null;
		return gamePiece;
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

	public Wall GetWall(Side s){
		return walls[s];
	}

	public bool IsSolidlyOccupied(){
		return !Occupy(null);
	}

	public bool Reserve ()
	{
		throw new NotImplementedException ();
	}

    public Cell(RoomManager room)
    {
		this.room = room;
	}
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public enum Side { top, right, bottom, left };
public class Cell {

    public int x { get; set; }
    public int y { get; set; }
	RoomManager room;

    public Dictionary<Side, Wall> walls = new Dictionary<Side, Wall>();

    public GamePiece gamePiece;

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
			}
		} catch (IndexOutOfRangeException e){
			return null;
		}
	}

	public Wall getWall(Side s){
		return walls[s];
	}

	public bool isOccupied(){
		if (gamePiece == null) return false;
		GamePiece g = gamePiece.containedPiece;
		while (g!= null){
		if (g.isSolid) return true;
			g=g.containedPiece;
		} return false;
	}

    public Cell(RoomManager room)
    {
		this.room = RoomManager;
	}
}

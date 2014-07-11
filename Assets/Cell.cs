using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public enum Side {none, top, right, bottom, left };
public class Cell {

    public int x { get; set; }
    public int y { get; set; }
	RoomManager room;

    private Dictionary<Side, Wall> walls = new Dictionary<Side, Wall>();
	public static Cell Get(int x, int y){
        if (x < 0 || x >= RoomManager.roomManager.Grid.Length
            || y < 0 || y >= RoomManager.roomManager.Grid[0].Length) return null;
		return RoomManager.roomManager.Grid[x][y];
	}

	public static Cell GetFromWorldPos(Vector2 worldPos){
		return GetFromWorldPos(worldPos.x, worldPos.y);
	}
	public static Cell GetFromWorldPos(float x, float y){
		int blockSize = Wall.blockSize;
        int originX = (int)Mathf.Floor(x / blockSize);
        int originY = (int)Mathf.Floor(y / blockSize);
        Debug.Log("adding: " + originX + " " + originY);
		return Cell.Get(originX, originY);
	}

	public Vector2 WorldPos(){return new Vector2(x*Wall.blockSize+Wall.halfBlock, y*Wall.blockSize+Wall.halfBlock);}

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
			gamePiece = piece;
			piece.cell = this;
			return true;
		} 
		GamePiece g = gamePiece;
		while(g.containedPiece!=null){
			if(g.isSolid)return false;
			g=g.containedPiece;
		}
		if (!g.onOccupy(piece))return false;
		piece.cell = this;
		return true;
	}
	public GamePiece DeOccupy(){
		GamePiece ret = gamePiece;
		gamePiece = null;
        return ret;
	}

	public Cell getNeighbour(Side s){
        switch (s)
        {
            case Side.bottom:
                return Cell.Get(x, y - 1);
            case Side.top:
                return Cell.Get(x, y + 1);
            case Side.left:
                return Cell.Get(x - 1, y);
            case Side.right:
                return Cell.Get(x + 1, y);
        }
        return null;
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

	public bool IsSolidlyOccupied(){
		return !Occupy(null);
	}
    private bool IsReserved = false;
	public bool Reserve()
	{
        if (IsReserved) return false;
        IsReserved = true;
        return true;
	}
    public void Unreserve()
    {
        IsReserved = false;
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

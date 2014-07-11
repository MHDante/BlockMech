using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public enum Side {none, top, right, bottom, left };
public class Cell {

    public int x { get; set; }
    public int y { get; set; }

    private Dictionary<Side, Wall> walls = new Dictionary<Side, Wall>();

	/// <summary>
	/// Returns The cell at the given grid position;
	/// </summary>
	/// <param name="x">X position.</param>
	/// <param name="Y">y position..</param>
	/// <returns>Returns null if given a value outside of the grid bounds</returns>
	public static Cell Get(int x, int y){
        if (x < 0 || x >= RoomManager.roomManager.Grid.Length
            || y < 0 || y >= RoomManager.roomManager.Grid[0].Length) return null;
		Cell ret = RoomManager.roomManager.Grid[x][y];
		if(x != ret.x || y != ret.y) throw new WTFException();
		return ret;
	}
	/// <summary>
	/// Returns The cell at the given position from Unity's Coordinate sustem;
	/// The coordinate system is zero based and given values will be floored.
	/// </summary>
	/// <param name="worldPos">Position(in world units).</param>
	/// <returns>Returns null if given a value outside of the grid bounds</returns>
	public static Cell GetFromWorldPos(Vector2 worldPos){
		return GetFromWorldPos(worldPos.x, worldPos.y);
	}
	/// <summary>
	/// Returns The cell at the given position from Unity's Coordinate sustem;
	/// The coordinate system is zero based and given values will be floored.
	/// </summary>
	/// <param name="x">X position.</param>
	/// <param name="Y">y position..</param>
	/// <returns>Returns null if given a value outside of the grid bounds</returns>
	public static Cell GetFromWorldPos(float x, float y){
		int blockSize = Wall.blockSize;
        int originX = (int)Mathf.Floor(x / blockSize);
        int originY = (int)Mathf.Floor(y / blockSize);
        Debug.Log("adding: " + originX + " " + originY);
		return Cell.Get(originX, originY);
	}
	/// <summary>
	/// Returns the Unity location of the center of the cell
	/// </summary>
	public Vector2 WorldPos(){return new Vector2(x*Wall.blockSize+Wall.halfBlock, y*Wall.blockSize+Wall.halfBlock);}

	/// <summary>
	/// The gamepiece currently occupying this cell. To get all of the gamepieces occupying this cell,
	/// call getPiecesOnCell().
	/// </summary>
	public GamePiece gamePiece{ get; set;}

	/// <summary>
	/// Returns a list of gamePieces on the cell. Starting from the bottom-most piece.
	/// </summary>
    public List<GamePiece> getPiecesOnCell()
    {
        List<GamePiece> list = new List<GamePiece>();
		GamePiece g = gamePiece;
        while (g != null)
        {
			list.Add(g);
			g = g.containedPiece;
        }
        return list;
    }

	/// <summary>
	/// Places the <param name="piece">piece</param> within this cell. If this cell is already occupied,
	/// tries to place the piece within the cell that is already there.
	/// </summary>
	/// <returns>Returns false if the piece was not able to move into the cell.</returns>
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
	/// <summary>
	/// Removes the contents of this cell;
	/// </summary>
	/// <returns>Returns the piece that was in this cell.</returns>
	public GamePiece DeOccupy(){
		GamePiece ret = gamePiece;
		gamePiece = null;
        return ret;
	}
	/// <summary>
	/// Returns the adjoining cell in the specified direction param name="piece">s</param>.
	/// </summary>
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
	/// <summary>
	/// Returns the adjoining wall in the specified direction param name="piece">s</param>.
	/// </summary>
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

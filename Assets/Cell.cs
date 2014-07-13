using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public enum Side {top, right, bottom, left };
public class Cell {

    public int x { get; set; }
    public int y { get; set; }
    public List<GamePiece> pieces { get; set; }
    public GamePiece this[int i] { get { return pieces[i]; } }

    private Dictionary<Side, Wall> walls = new Dictionary<Side, Wall>();

    public Cell(int x, int y)
    {
        this.x = x;
        this.y = y;
        walls = new Dictionary<Side, Wall>();
        foreach (Side s in Enum.GetValues(typeof(Side)))
        {
            walls[s] = null;
        }
        pieces = new List<GamePiece>();
    }
    public bool HasPiece()
    {
        return pieces.Count > 0;
    }
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
    //private GamePiece _g;
    //public GamePiece gamePiece{ 
    //    get{return _g;} set{
    //        if (value == null)
    //            Debug.Log("Who?");
    //        _g = value;
    //    }
    //}
    //-----
    

	/// <summary>
	/// Returns a list of gamePieces on the cell. Starting from the bottom-most piece.
	/// </summary>
    //public List<GamePiece> getPiecesOnCell()
    //{
    //    List<GamePiece> list = new List<GamePiece>();
	//	GamePiece g = gamePiece;
    //    while (g != null)
    //    {
	//		list.Add(g);
	//		g = g.containedPiece;
    //    }
    //    return list;
    //}

	/// <summary>
	/// Places the <param name="piece">piece</param> within this cell. If this cell is already occupied,
	/// tries to place the piece within the cell that is already there.
	/// </summary>
	/// <returns>Returns false if the piece was not able to move into the cell.</returns>
	//public bool Occupy(GamePiece piece){
    //    if (IsReserved)
    //    {
    //        Debug.Log("God. I am not a religious man. But Today I come to you In a time of need:" +
    //        "Please let this return true..." + IsReserved);
    //        return false;
    //    }
	//	if(gamePiece == null){
	//		gamePiece = piece;
	//		piece.cell = this;
	//		return true;
	//	} 
	//	GamePiece g = gamePiece;
	//	while(g.containedPiece!=null){
	//		if(g.isSolid)
    //            return false;
	//		g=g.containedPiece;
	//	}
	//	if (!g.onOccupy(piece))
    //        return false;
	//	piece.cell = this;
	//	return true;
	//}	
    //-----------
    public bool Occupy(GamePiece piece)
    {
        if (pieces.Contains(piece)) return false;
        if (IsSolidlyOccupied()) return false;
        foreach (GamePiece p in pieces)
        {
            if (!p.canBeOccupiedBy(piece)) return false;
        }
        foreach (GamePiece p in pieces)
        {
            p.onOccupy(piece); 
        }
        pieces.Add(piece);
        piece.cell = this;
        return true;
    }

    private Dictionary<int, GamePiece> occupationQueue = new Dictionary<int, GamePiece>();
    public void QueuedOccupy(int Zposition, GamePiece piece)
    {
        if (Zposition > pieces.Count)
            occupationQueue[Zposition] = piece;
        else
        {
            pieces.Add(piece);
            foreach (int i in occupationQueue.Keys)
                QueuedOccupy(i, occupationQueue[i]);
        }
    }


	/// <summary>
	/// Removes the contents of this cell;
	/// </summary>
	/// <returns>Returns the piece that was in this cell.</returns>
    //public GamePiece Empty(){
    //	GamePiece ret = gamePiece;
    //	gamePiece = null;
    //    if (ret != null) 
    //    ret.cell = null;
    //    return ret;
    //}
    //-----
    public List<GamePiece> Empty()
    {
        if (pieces.Count <= 1) return new List<GamePiece>();
        return pieces[0].DetatchWithChilren();
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
    //public GamePiece firstSolid()
    //{
    //    GamePiece g = gamePiece;
    //    while (g != null)
    //    {
    //        if (g.isSolid) return g;
    //        g = g.containedPiece;
    //    }
    //    return null;
    //}
    //---------
    public GamePiece firstSolid()
    {
        foreach(GamePiece piece in pieces)
        {
            if (piece.isSolid) return piece;
        }
        return null;
    }
    public bool setWall(Side s, Wall w)
    {
        walls[s] = w;
        return true;
    }

    //public bool IsSolidlyOccupied(){
    //	if (IsReserved) return true;
    //    if (gamePiece == null)
    //    {
    //        return false;
    //    }
    //    GamePiece g = gamePiece;
    //    while (g != null)
    //    {
    //        if (g.isSolid) return true;
    //        g = g.containedPiece;
    //    }
    //    return false;
    //}
    //------
    public bool IsSolidlyOccupied()
    {
        if (IsReserved) return true;
        return HasPiece() && pieces.Any(p => p.isSolid);
    }
    private bool IsReserved = false;
	public bool Reserve()
	{
		if (IsSolidlyOccupied()) return false;
        IsReserved = true;
		return IsReserved;
	}
    public void Unreserve()
    {
        IsReserved = false;
    }
}

using UnityEngine;
using System.Collections;
using System.Linq;
using System;
[ExecuteInEditMode]
public class RoomManager : MonoBehaviour {
    public static RoomManager roomManager;

    public const int gridWidth = 16;
    public const int gridHeight = 12;

    public Cell[][] Grid;


    void Awake() {
        roomManager = this;
        Grid = new Cell[gridWidth][];
        for (int i = 0; i < gridWidth; i++)
        {
            Grid[i] = new Cell[gridHeight];
            for(int j = 0; j < gridHeight; j++){
                Grid[i][j] = new Cell(this, i, j);
            }
        }

        GameObject[] walls = GameObject.FindGameObjectsWithTag("Wall");
		foreach (GameObject wallobj in walls)
        {
			Wall wall  = wallobj.GetComponent<Wall>();
			if(wall != null){
				if(((Vector2)wall.transform.position).isWithinGrid()){
                    AddWall(wall);
				}
				else{
                    Debug.Log("Wall was found out of Grid Range @ " + wall.transform.position);
				}
			}
        }
    }
    public void PrintWallAmount()
    {

        int count = 0;
        for(int i = 0; i < Grid.Length; i++)
        {
            for(int j = 0; j < Grid[0].Length; j++)
            {
                foreach(Side s in Enum.GetValues(typeof(Side)))
                {
                    if (Grid[i][j].getWall(s) != null)
                    {
                        Debug.Log(i + " : " + j + " side: " + s.ToString());
                        count++;

                    }
                }
            }
        }
        Debug.Log(string.Format("Total refs: {0}\nActual: {1}", count, (count / 2)));
    }
    public void AddPiece(GameObject piece, PieceType piecetype)
    {
        GamePiece gamePiece = piece.GetComponent<Player>();
        if (gamePiece == null)
        {
            gamePiece = piece.AddComponent<Player>();
            gamePiece.piecetype = piecetype;
        }
		Cell cell = Cell.GetFromWorldPos(piece.transform.position);
        if (cell != null)
        {
            var list = cell.getPiecesOnCell();
            if (list.Select(gpiece => gpiece.piecetype).Contains(piecetype))
            {
                return;
            }
            bool success = cell.Occupy(gamePiece);
            //do something if the cell was successfully placed.
        }

    }

    //public void AddWall(Wall wall, Side side)
    //{
    //	float x = wall.transform.position.x;
    //	float y = wall.transform.position.y;
    //
    //	if (wall.orientation == Wall.Orientation.Horizontal){
    //		if (Cell.GetFromWorldPos(x, y)!= null) Cell.GetFromWorldPos(x,y).setWall(side,wall);
    //	} else if(wall.orientation == Wall.Orientation.Vertical){
    //		if (Cell.GetFromWorldPos(x, y )!= null) Cell.GetFromWorldPos(x,y).setWall(side,wall);
    //	}
    //}
	// Use this for initialization
	void Start () {
	}
	// Update is called once per frame
	void Update () {
        if(!Application.isPlaying && roomManager == null)
        { roomManager = this; Awake(); }
	}
    public void RemovePiece(Cell target, bool destroyChildren = false)//, PieceType piece)
    {

		if (target != null)
        {
			var gamepieces = target.getPiecesOnCell();
			if (target.gamePiece == null || gamepieces.Count == 0) return;
            GamePiece g = gamepieces.Last();

            g.Destroy(destroyChildren);
        }
    }

	public void RemoveWall(Vector2 position)
    {
        Side side; Wall.Orientation orient;
        Utils.WorldToWallPos(position, out side, out orient);
        Cell cell = Cell.GetFromWorldPos(position);

        if (cell == null) { Debug.Log("Stay inside the grid!"); return; }
		if (cell.getWall(side) != null)
        {
			DestroyImmediate(cell.getWall(side).gameObject);
        }
		cell.setWall(side, null);
		Cell neighbour = cell.getNeighbour(side);
		if (neighbour != null){
			neighbour.setWall(Utils.opposite(side), null);
	    }
	}
    public void AddWall(Wall wall)
    {
        Side side; Wall.Orientation orient;
        Utils.WorldToWallPos(wall.transform.position, out side, out orient);
        Cell cell = Cell.GetFromWorldPos(wall.transform.position);

        if (cell == null) { Debug.Log("Stay inside the grid!"); return; }
        
        if (cell.getWall(side) != null)
        {
            RemoveWall(wall.transform.position);
        }
        cell.setWall(side, wall);
        Cell neighbour = cell.getNeighbour(side);
        if (neighbour != null)
        {
            neighbour.setWall(Utils.opposite(side), wall);
        }

    }
}

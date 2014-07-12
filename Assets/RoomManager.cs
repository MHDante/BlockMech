using UnityEngine;
using System.Collections;
using System.Linq;
using System;
using System.Collections.Generic;
[ExecuteInEditMode]
public class RoomManager : MonoBehaviour {
    public static RoomManager roomManager;
    public static Dictionary<PieceType, GameObject> pieceGameObjects;
    public static Dictionary<PieceType, GameObject> pieceParents;
    public static Dictionary<PieceType, Type> pieceTypes;

    public const int gridWidth = 16;
    public const int gridHeight = 12;

    public Cell[][] Grid;
    public Player player;

    void InitializeDictionaries()
    {
        if (RoomManager.pieceGameObjects == null || RoomManager.pieceTypes == null)
        {
            RoomManager.pieceTypes = new Dictionary<PieceType, Type>()
            {
                { PieceType.wall, typeof(Wall) },
                { PieceType.door, typeof(Door) },
                { PieceType.player, typeof(Player) },
                { PieceType.end, typeof(End) },
                { PieceType.button, typeof(Button) },
                { PieceType.switcH, typeof(Switch) },
                { PieceType.key, typeof(Key) },
                { PieceType.keyhole,  typeof(Keyhole) },
                { PieceType.teleport, typeof(Teleport) },
                { PieceType.tile, typeof(Tile) },
                { PieceType.trap, typeof(Trap) },
                { PieceType.antitrap, typeof(Antitrap) },
            };
            RoomManager.pieceGameObjects = new Dictionary<PieceType, GameObject>()
            {
                { PieceType.wall, Resources.Load<GameObject>("Prefabs/Wall")},
                { PieceType.door, Resources.Load<GameObject>("Prefabs/Wall")},//also wall, door script added later
                { PieceType.player, Resources.Load<GameObject>("Prefabs/player")},
                { PieceType.end, Resources.Load<GameObject>("Prefabs/end")},
                { PieceType.button, Resources.Load<GameObject>("Prefabs/button")},
                { PieceType.switcH, Resources.Load<GameObject>("Prefabs/button")},//also button, switcH script added later
                { PieceType.key, Resources.Load<GameObject>("Prefabs/key")},
                { PieceType.keyhole, Resources.Load<GameObject>("Prefabs/Keyhole")},
                { PieceType.teleport, Resources.Load<GameObject>("Prefabs/teleport")},
                { PieceType.tile, Resources.Load<GameObject>("Prefabs/tile")},
                { PieceType.trap, Resources.Load<GameObject>("Prefabs/trap")},
                { PieceType.antitrap, Resources.Load<GameObject>("Prefabs/anti-trap")},
            };
            RoomManager.pieceParents = new Dictionary<PieceType, GameObject>();
        }
    }

    void Awake() {
        roomManager = this;
        Grid = new Cell[gridWidth][];
        for (int i = 0; i < gridWidth; i++)
        {
            Grid[i] = new Cell[gridHeight];
            for(int j = 0; j < gridHeight; j++){
                Grid[i][j] = new Cell(i, j);
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

        InitializeDictionaries();
    }
    public void PrintWallAmount()
    {
        int count = 0;
        for(int i = 0; i < Grid.Length; i++)
        {
            for(int j = 0; j < Grid[0].Length; j++)
            {
                //foreach(Side s in Enum.GetValues(typeof(Side)))
                //{
                //    if (Grid[i][j].getWall(s) != null)
                //    {
                //        Debug.Log(i + " : " + j + " side: " + s.ToString());
                //        count++;
                //
                //    }
                //}
                if (Cell.Get(i, j).gamePiece != null)
                    Debug.Log("Found : " + Cell.Get(i, j).gamePiece + " @ " + i + " , " + j);
            }
        }
        Debug.Log(string.Format("Total refs: {0}\nActual: {1}", count, (count / 2)));
    }
    public void AddPiece(GameObject gameobject, PieceType piecetype)
    {
        GamePiece gamePiece;
        Type t = pieceTypes[piecetype];
        gamePiece = (GamePiece)gameobject.GetComponent(t);
        if (gamePiece == null)
        {
            gamePiece = (GamePiece)gameobject.AddComponent(t);
        }
        gamePiece.piecetype = piecetype;
        if (t == typeof(Player))
        {
            player = (Player)gamePiece;
        }


        //if (piecetype == PieceType.player)
        //{
        //    gamePiece = gameobject.GetComponent<Player>();
        //    if (gamePiece == null) gamePiece = gameobject.AddComponent<Player>();
        //    RoomManager.roomManager.player = (Player)gamePiece;
        //    gamePiece.piecetype = piecetype;
        //}
        //else
        //{
        //    gamePiece = gameobject.GetComponent<Placeholder>();
        //    if (gamePiece == null) gamePiece = gameobject.AddComponent<Placeholder>();
        //    gamePiece.piecetype = piecetype;
        //}
		//Cell cell = Cell.GetFromWorldPos(piece.transform.position);
        //if (cell != null)
        //{
        //    var list = cell.getPiecesOnCell();
        //    if (list.Select(gpiece => gpiece.piecetype).Contains(piecetype))
        //    {
        //        return;
        //    }
        //    bool success = cell.Occupy(gamePiece);
        //    //do something if the cell was successfully placed.
        //}

    }
	void Start () {
        if (Application.isPlaying)
        {
            GameObject preexisting = GameObject.Find("Indicator");
            if (preexisting != null) DestroyImmediate(preexisting);
        }
	}
	void Update () {
        if(!Application.isPlaying && roomManager == null)
        { roomManager = this; Awake(); }
	}
    public void RemoveTopPiece(Cell target, bool destroyChildren = false)//, PieceType piece)
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

        if (cell == null) { return; }
		if (cell.getWall(side) != null)
        {
            if (cell.getWall(side).gameObject) DestroyImmediate(cell.getWall(side).gameObject);
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

        if (cell == null) {  return; }
        
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

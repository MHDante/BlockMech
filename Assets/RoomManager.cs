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
                { PieceType.door, Resources.Load<GameObject>("Prefabs/Door")},//also wall, door script added later
                { PieceType.player, Resources.Load<GameObject>("Prefabs/player")},
                { PieceType.end, Resources.Load<GameObject>("Prefabs/end")},
                { PieceType.button, Resources.Load<GameObject>("Prefabs/button")},
                { PieceType.switcH, Resources.Load<GameObject>("Prefabs/switch")},//also button, switcH script added later
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
        List<GameObject> walls = GameObject.FindObjectsOfType<Wall>().Select(w => w.gameObject).ToList();

		foreach (GameObject wallobj in walls)
        {
            //if (wallobj.GetComponent<Door>() != null) Debug.Log("FOUND DOOR");
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

        if (player == null)
        {
            Debug.LogWarning("Level needs <color=magenta>player</color>, add with <color=magenta>PuzzleMaker plugin</color>");
        }
    }
    public void PrintWallAmount()
    {
        int count = 0;
        for(int i = 0; i < Grid.Length; i++)
        {
            for(int j = 0; j < Grid[0].Length; j++)
            {
                if (Cell.Get(i, j).pieces.Count != 0)
                    Debug.Log("Found : " + Cell.Get(i, j).pieces[0] + " @ " + i + " , " + j);
            }
        }
        Debug.Log(string.Format("Total refs: {0}\nActual: {1}", count, (count / 2)));
    }
    public List<GamePiece> GetPiecesOfColor(ColorSlot colorslot, Type gamePieceType = null)
    {
        List<GamePiece> list = new List<GamePiece>();
        Func<GamePiece, bool> typeTest;
        if (gamePieceType != null) typeTest = (p) => p.GetType() == gamePieceType;
        else typeTest = (p) => true;
        for (int i = 0; i < Grid.Length; i++)
        {
            for (int j = 0; j < Grid[0].Length; j++)
            {
                Cell cell = Grid[i][j];
                if (cell == null) continue;
                var cellList = cell.pieces.ToList();
                foreach(var gamepiece in cellList)
                {
                    if (typeTest(gamepiece) && !list.Contains(gamepiece) && gamepiece.colorslot == colorslot)
                    {
                        list.Add(gamepiece);
                    }
                }
            }
        }
        return list;
    }
    public List<Door> GetDoorsOfColor(ColorSlot colorslot)
    {
        List<Door> list = new List<Door>();
        for (int i = 0; i < Grid.Length; i++)
        {
            for (int j = 0; j < Grid[0].Length; j++)
            {
                Cell cell = Grid[i][j];
                if (cell == null) continue;
                foreach (var wall in cell.walls.Values)
                {
                    if (wall != null && wall is Door)
                    {
                        Door door = (Door)wall;
                        if (door.colorslot == colorslot && !list.Contains(door))
                        {
                            list.Add(door);
                        }
                    }
                }
            }
        }
        return list;
    }
    public void AddPiece(GameObject gameobject, PieceType piecetype, ColorSlot colorslot)
    {
        GamePiece gamePiece;
        Type t = pieceTypes[piecetype];
        gamePiece = (GamePiece)gameobject.GetComponent(t);
        if (gamePiece == null)
        {
            gamePiece = (GamePiece)gameobject.AddComponent(t);
        }
        gamePiece.piecetype = piecetype;
        gamePiece.SetColorSlot(colorslot);
        //gameobject.name = 
        if (t == typeof(Player))
        {
            player = (Player)gamePiece;
        }
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
    public void RemoveTopPiece(Cell target)
    {
		if (target != null)
        {
			var gamepieces = target.pieces.ToList();
			if (!target.HasPiece()) return;
            GamePiece g = gamepieces.Last();
            g.Destroy();
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
        if (cell == null) return;
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

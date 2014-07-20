using UnityEngine;
using System.Collections;
using System.Linq;
using System;
using System.Collections.Generic;
[ExecuteInEditMode]
public class RoomManager : MonoBehaviour {
    public static RoomManager roomManager;
    public static Dictionary<PieceType, GameObject> pieceGameObjects;
    public static GameObject masterParent;
    public static Dictionary<PieceType, GameObject> pieceParents;
    public static Dictionary<PieceType, Type> pieceTypes;

    public const int gridWidth = 16;
    public const int gridHeight = 12;

    public Cell[][] Grid;
    public Player player;

    public enum ButtonOptions
    {
        ActivateOnAllPushed,
        ActivateOnOnePushed,
    }
    public ButtonOptions buttonOptions = ButtonOptions.ActivateOnAllPushed;

    void InitializeDictionaries()
    {
        if (RoomManager.pieceGameObjects == null || RoomManager.pieceTypes == null)
        {
            RoomManager.pieceTypes = new Dictionary<PieceType, Type>()
            {
                { PieceType.Wall, typeof(Wall) },
                { PieceType.Block, typeof(Block) },
                { PieceType.Player, typeof(Player) },
                { PieceType.End, typeof(End) },
                { PieceType.Button, typeof(Button) },
                { PieceType.Switch, typeof(Switch) },
                { PieceType.Key, typeof(Key) },
                { PieceType.Keyhole,  typeof(Keyhole) },
                { PieceType.Teleport, typeof(Teleport) },
                { PieceType.Tile, typeof(Tile) },
                { PieceType.Trap, typeof(Trap) },
                { PieceType.AntiTrap, typeof(Antitrap) },
            };
            RoomManager.pieceGameObjects = new Dictionary<PieceType, GameObject>()
            {
                { PieceType.Wall, Resources.Load<GameObject>("Prefabs/Wall")},
                { PieceType.Block, Resources.Load<GameObject>("Prefabs/octogon")},
                { PieceType.Player, Resources.Load<GameObject>("Prefabs/player")},
                { PieceType.End, Resources.Load<GameObject>("Prefabs/end")},
                { PieceType.Button, Resources.Load<GameObject>("Prefabs/button")},
                { PieceType.Switch, Resources.Load<GameObject>("Prefabs/switch")},//also button, switcH script added later
                { PieceType.Key, Resources.Load<GameObject>("Prefabs/key")},
                { PieceType.Keyhole, Resources.Load<GameObject>("Prefabs/Keyhole")},
                { PieceType.Teleport, Resources.Load<GameObject>("Prefabs/teleport")},
                { PieceType.Tile, Resources.Load<GameObject>("Prefabs/tile")},
                { PieceType.Trap, Resources.Load<GameObject>("Prefabs/trap")},
                { PieceType.AntiTrap, Resources.Load<GameObject>("Prefabs/anti-trap")},
            };
            RoomManager.pieceParents = new Dictionary<PieceType, GameObject>();
        }
    }

    void Awake() {
        if (masterParent == null) masterParent = GameObject.Find("Puzzle_Pieces");
        if (masterParent == null) masterParent = new GameObject("Puzzle_Pieces");

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
    public List<T> GetPiecesOfType<T>() where T : GamePiece
    {
        List<T> list = new List<T>();
        for (int i = 0; i < Grid.Length; i++)
        {
            for (int j = 0; j < Grid[0].Length; j++)
            {
                Cell cell = Grid[i][j];
                if (cell == null) continue;
                var cellList = cell.pieces.ToList();
                foreach (var gamepiece in cellList)
                {
                    if (gamepiece is T && !list.Contains((T)gamepiece))
                    {
                        list.Add((T)gamepiece);
                    }
                }
            }
        }
        return list;
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
    public List<Wall> GetDoorsOfColor(ColorSlot colorslot)
    {
        List<Wall> list = new List<Wall>();
        for (int i = 0; i < Grid.Length; i++)
        {
            for (int j = 0; j < Grid[0].Length; j++)
            {
                Cell cell = Grid[i][j];
                if (cell == null) continue;
                foreach (var wall in cell.walls.Values)
                {
                    if (wall != null && wall.isDoor)
                    {
                        if (wall.colorslot == colorslot && !list.Contains(wall))
                        {
                            list.Add(wall);
                        }
                    }
                }
            }
        }
        return list;
    }

    public void RefreshColorFamily(ColorSlot colorslot)
    {
        if (buttonOptions == ButtonOptions.ActivateOnAllPushed)
        {
            bool allSatisfied = true;
            List<GamePiece> pieces = GetPiecesOfColor(colorslot);
            foreach (GamePiece piece in pieces)
            {
                if (piece is Triggerable)
                {
                    Triggerable t = (Triggerable)piece;
                    if (!t.IsTriggered) allSatisfied = false;
                }
            }
            List<Wall> doors = GetDoorsOfColor(colorslot);
            foreach (Wall door in doors)
            {
                if (allSatisfied) door.Open();
                else door.Close();
            }
        }
        else if (buttonOptions == ButtonOptions.ActivateOnOnePushed)
        {
            bool oneSatisfied = false;
            List<GamePiece> pieces = GetPiecesOfColor(colorslot);
            foreach (GamePiece piece in pieces)
            {
                if (piece is Triggerable)
                {
                    Triggerable t = (Triggerable)piece;
                    if (t.IsTriggered)
                    {
                        oneSatisfied = true;
                        break;
                    }
                }
            }
            List<Wall> doors = GetDoorsOfColor(colorslot);
            foreach (Wall door in doors)
            {
                if (oneSatisfied) door.Open();
                else door.Close();
            }
        }
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
        foreach (ColorSlot val in Enum.GetValues(typeof(ColorSlot)))
        {
            RefreshColorFamily(val);
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
        Side side; Orientation orient;
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
        Side side; Orientation orient;
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

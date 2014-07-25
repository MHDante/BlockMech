using UnityEngine;
using System.Collections;
using System.Linq;
using System;
using System.Collections.Generic;
[ExecuteInEditMode]
public class RoomManager : MonoBehaviour {
    public static RoomManager roomManager;
    public static GameObject masterParent;



    public static Dictionary<Type, GameObject> pieceParents = new Dictionary<Type,GameObject>();
    public Cell[][] Grid;
    public Player player;

    public enum ButtonOptions
    {
        ActivateOnAllPushed,
        ActivateOnOnePushed,
    }
    public ButtonOptions buttonOptions = ButtonOptions.ActivateOnAllPushed;

    void Awake() {
        if (Application.isPlaying)
        {
            GameObject preexisting = GameObject.Find("Indicator");
            if (preexisting != null) DestroyImmediate(preexisting);
        }
        if (masterParent == null) masterParent = GameObject.Find("Puzzle_Pieces");
        if (masterParent == null) masterParent = new GameObject("Puzzle_Pieces");

        roomManager = this;
        Grid = new Cell[Values.gridWidth][];
        for (int i = 0; i < Values.gridWidth; i++)
        {
            Grid[i] = new Cell[Values.gridHeight];
            for (int j = 0; j < Values.gridHeight; j++)
            {
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
        //if (player == null)
        //{
        //    Debug.LogWarning("Level needs <color=magenta>player</color>, add with <color=magenta>PuzzleMaker plugin</color>");
        //}
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

    Dictionary<ColorSlot, bool> colorActivation = Enum.GetValues(typeof(ColorSlot)).OfType<ColorSlot>().ToDictionary(key => key, key => false);
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
            if (allSatisfied != colorActivation[colorslot])
            {
                foreach (Wall door in GetDoorsOfColor(colorslot))
                {
                    door.Activate();
                }
            }
            colorActivation[colorslot] = allSatisfied;

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
            if (oneSatisfied != colorActivation[colorslot])
            {
                List<Wall> doors = GetDoorsOfColor(colorslot);
                foreach (Wall door in doors)
                {
                    door.Activate();
                }
            }
            colorActivation[colorslot] = oneSatisfied;
        }
    }
    public void AddPiece(GameObject gameobject, Type t, ColorSlot colorslot)
    {
        if (!t.IsSubclassOf(typeof(GamePiece))) throw new System.Exception("Tried to add a non-GamePiece to a Cell");
        GamePiece gamePiece;
        gamePiece = (GamePiece)gameobject.GetComponent(t);
        if (gamePiece == null)
        {
            gamePiece = (GamePiece)gameobject.AddComponent(t);
        }
        gamePiece.SetColorSlot(colorslot);
        //gameobject.name = 
        if (t == typeof(Player))
        {
            player = (Player)gamePiece;
        }
    }
	void Start () {

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

        if (cell == null)
        {
            cell = FindBorderCell(position);
            if (cell != null)
            {
                side = side.opposite();
            }
            else
            {
                return;
            }
        }
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
        if (cell == null)
        {
            cell = FindBorderCell(wall.transform.position);
            if (cell != null)
            {
                side = side.opposite();
            }
            else
            {
                return;
            }
        }
        Wall alreadyThere = cell.getWall(side);
        if (alreadyThere != null)
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
    public Cell FindBorderCell(Vector3 position)
    {
        Cell cell = null;
        int x = Cell.GetCellX(position.x);
        int y = Cell.GetCellX(position.y);
        if (position.x / Values.blockSize == Grid.Length)
        {
            cell = Grid[x - 1][y];
        }
        else if (position.y / Values.blockSize == Grid[0].Length)
        {
            cell = Grid[x][y - 1];
        }
        return cell;
    }
}

﻿using UnityEngine;
using System.Collections;
using OrbItUtils;

using System.Linq;
using System;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
[ExecuteInEditMode]
public class RoomManager : MonoBehaviour {
    public static RoomManager roomManager;
    public static GameObject masterParent;
    public static Dictionary<Type, GameObject> pieceParents = new Dictionary<Type, GameObject>();
    public Cell[][] Grid;
    public Player player;

    public enum ButtonOptions
    {
        ActivateOnAllPushed,
        ActivateOnOnePushed,
    }
    public ButtonOptions buttonOptions = ButtonOptions.ActivateOnAllPushed;
    public static List<Type> PieceTypeList;
    static RoomManager()
    {
        PieceTypeList = new List<Type>();
        PieceTypeList.Add(typeof(Wall));
        
        foreach (Type t in typeof(GamePiece).Assembly.GetTypes().Where(type => type.IsSubclassOf(typeof(GamePiece)) && !type.IsAbstract))
        {
            PieceTypeList.Add(t);
        }
    }

    public static bool IsWithinGrid(Vector2 worldPos)
    {
        if (worldPos.x > Values.gridWidth * Values.blockSize || worldPos.x < 0) return false;
        if (worldPos.y > Values.gridHeight * Values.blockSize || worldPos.y < 0) return false;
        return true;
    }

    void Awake() {
        roomManager = this;
        if (Application.isPlaying)
        {
            GameObject preexisting = GameObject.Find("Indicator");
            if (preexisting != null) DestroyImmediate(preexisting);
        }
        if (masterParent == null) masterParent = GameObject.Find("Puzzle_Pieces");
        if (masterParent == null) masterParent = new GameObject("Puzzle_Pieces");
        Grid = new Cell[Values.gridWidth][];
        for (int i = 0; i < Values.gridWidth; i++)
        {
            Grid[i] = new Cell[Values.gridHeight];
            for (int j = 0; j < Values.gridHeight; j++)
            {
                Grid[i][j] = new Cell(i, j);
            }
        }
        List<GameObject> walls = FindObjectsOfType<Wall>().Select(w => w.gameObject).ToList();

		foreach (GameObject wallobj in walls)
        {
            //if (wallobj.GetComponent<Door>() != null) Debug.Log("FOUND DOOR");
			Wall wall  = wallobj.GetComponent<Wall>();
			if(wall != null){
				if((IsWithinGrid(wall.transform.position))){
                    AddWall(wall);
				}
				else{
                    Debug.Log("Wall was found out of Grid Range @ " + wall.transform.position);
				}
			}
        }
        try
        {
            FileWrite.DeserializationCallback();
        }
        catch (Exception e)
        {
            Debug.Log("Exception : " + e);
            throw e;
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
                        if (wall.colorSlot == colorslot && !list.Contains(wall))
                        {
                            list.Add(wall);
                        }
                    }
                }
            }
        }
        return list;
    }
    //Dictionary<ColorSlot, bool> colorActivation = Enum.GetValues(typeof(ColorSlot)).OfType<ColorSlot>().ToDictionary(key => key, key => false);
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
            //if (allSatisfied != colorActivation[colorslot])
            //{
            //    foreach (Wall door in GetDoorsOfColor(colorslot))
            //    {
            //        door.Activate();
            //    }
            //}
            //colorActivation[colorslot] = allSatisfied;
            var doors = GetDoorsOfColor(colorslot);
            foreach(Wall door in doors)
            {
                if (allSatisfied)
                {
                    door.Activate();
                }
                else
                {
                    door.Deactivate();
            }
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
            //if (oneSatisfied != colorActivation[colorslot])
            //{
            //    List<Wall> doors = GetDoorsOfColor(colorslot);
            //    foreach (Wall door in doors)
            //    {
            //        door.Activate();
            //    }
            //}
            //colorActivation[colorslot] = oneSatisfied;
            var doors = GetDoorsOfColor(colorslot);
            foreach (Wall door in doors)
            {
                if (oneSatisfied)
                {
                    door.Activate();
                }
                else
                {
                    door.Deactivate();
                }
            }
        }
    }
    //Used to be in PuzzleMaker
    public GamePiece SpawnPiece(Type piece, Cell target, ColorSlot colorslot = ColorSlot.None)
    {
        if (target.IsSolidlyOccupied())
        {
            Debug.LogError("Could not spawn: " + piece.Name + " at " + target.ToString() + " As it was occupied");
            return null;
        }
        GameObject parent = GetPieceParent(piece);
        GameObject obj = CreatePrefabSafe(piece);
        obj.transform.position = target.WorldPos();
        obj.transform.parent = parent.transform;

        GamePiece gp = roomManager.AddPiece(obj, piece, colorslot);
        gp.cell = target;
        return gp;
    }
    public void SpawnPlayer(Cell target)
    {
        if (roomManager.player == null)
        {
            GameObject obj = CreatePrefabSafe(typeof(Player));
            obj.transform.position = target.WorldPos();
            roomManager.AddPiece(obj, typeof(Player), ColorSlot.None);
        }
        else
        {
            roomManager.player.TeleportTo(target);
        }
    }
    //public void SpawnWall
    public Wall SpawnWall(Cell target, Side side, ColorSlot colorslot)
    {
        Orientation orient = (side == Side.top || side == Side.bottom) ? Orientation.Horizontal : Orientation.Vertical;
        GameObject wallobj = CreatePrefabSafe(typeof(Wall));
        wallobj.transform.position = target.WorldPos() + Cell.SideOffset(side);
        wallobj.transform.rotation = orient == Orientation.Vertical ? Quaternion.identity : Quaternion.Euler(0, 0, 90);
        wallobj.transform.parent = GetPieceParent(typeof(Wall)).transform;
        Wall wall = null;
        wall = wallobj.GetComponent<Wall>();
        if (wall == null) throw new WTFException();
        wall.orientation = orient;
        wall.SetColorSlot(colorslot);
        AddWall(wall);
        return wall;
    }

    public GameObject CreatePrefabSafe(Type type)
    {
#if UNITY_EDITOR
        return (GameObject)PrefabUtility.InstantiatePrefab(GamePiece.GetPrefab(type));
#else
        return (GameObject)Instantiate(GamePiece.GetPrefab(type));
#endif
    }

    public static GameObject GetPieceParent(Type piece)
    {
        if (!pieceParents.ContainsKey(piece) || pieceParents[piece] == null)
        {
            GameObject preexisting = GameObject.Find(piece.ToString().UppercaseFirst() + " Group");
            if (preexisting != null && preexisting.transform.parent == masterParent.transform)
            {
                pieceParents[piece] = preexisting;
            }
            else
            {
                pieceParents[piece] = new GameObject();
                pieceParents[piece].name = piece.ToString().UppercaseFirst() + " Group";
                pieceParents[piece].transform.parent = masterParent.transform;
        }
    }
        return pieceParents[piece];
    }

    public GamePiece AddPiece(GameObject gameobject, Type t, ColorSlot colorslot)
    {
        if (!t.IsSubclassOf(typeof(GamePiece))) throw new Exception("Tried to add a non-GamePiece to a Cell");
        GamePiece gamePiece;
        gamePiece = (GamePiece)gameobject.GetComponent(t);
        if (gamePiece == null)
        {
            gamePiece = (GamePiece)gameobject.AddComponent(t);
        }
        gamePiece.SetColorSlot(colorslot);
        if (t == typeof(Player))
        {
            player = (Player)gamePiece;
        }
        return gamePiece;
    }
	void Start () {
        RefreshColorFamilyAll();
        //Debug.Log("Roommanager");
	}
    public void RefreshColorFamilyAll()
    {
        foreach (ColorSlot val in Enum.GetValues(typeof(ColorSlot)))
        {
            RefreshColorFamily(val);
        }
	}
	void Update () {
        if(!Application.isPlaying && roomManager == null)
        { roomManager = this; Awake(); }
	}
    public GamePiece RemoveTopPiece(Cell target)
    {
		if (target != null)
        {
			var gamepieces = target.pieces.ToList();
			if (!target.HasPiece()) return null;
            GamePiece g = gamepieces.Last();
            g.Destroy();
            return g;
        }
        return null;
    }

	public Wall RemoveWall(Vector2 position)
    {
        Side side; Orientation orient;
        Wall.WorldToWallPos(position, out side, out orient);
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
                return null;
            }
        }
        Wall wall = cell.getWall(side);
		if (wall != null)
        {
            if (wall.gameObject) DestroyImmediate(wall.gameObject);
        }
		cell.setWall(side, null);
		Cell neighbour = cell.getNeighbour(side);
		if (neighbour != null){
			neighbour.setWall(side.opposite(), null);
	    }
        return wall;
	}
    public void AddWall(Wall wall)
    {
        Side side; Orientation orient;
        Wall.WorldToWallPos(wall.transform.position, out side, out orient);

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
            neighbour.setWall(side.opposite(), wall);
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

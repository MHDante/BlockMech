using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System;

public class PuzzleMaker : EditorWindow
{
    public PieceType selectedPiece = PieceType.wall;
    ColorSlot colorslot = ColorSlot.none;
    Color spawnColor = Color.white;
    Vector2 MousePos;
    bool LeftClick;
    bool RightClick;
	bool Active;
    GameObject Indicator;
    //GameObject WallParent;
    [MenuItem("Window/PuzzleMaker")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(PuzzleMaker));
    }
    void OnEnable()
    {        
        SceneView.onSceneGUIDelegate += OnUpdate;
        EditorApplication.hierarchyWindowItemOnGUI += delegate(int instanceID, Rect selectionRect)
        {
            if (Event.current != null && selectionRect.Contains(Event.current.mousePosition)
                && Event.current.button == 0 && Event.current.type <= EventType.MouseDown)
            {
                GameObject clickedObject = EditorUtility.InstanceIDToObject(instanceID) as GameObject;

                if (clickedObject){ Active = false; }
            }
        };
    }
	void OnDisable()
    {
		SceneView.onSceneGUIDelegate -= OnUpdate;
	}
    
    void UpdateIndicator(bool withinGrid)
    {
        if (Indicator == null)
        {
            SetIndicator();
        }
        else
        {
            foreach (var renderer in Indicator.GetComponentsInChildren<SpriteRenderer>())
            if (renderer != null)
            {
                if (withinGrid)
                {
                    if (!renderer.enabled) renderer.enabled = true;
                    if (selectedPiece != PieceType.wall && renderer.gameObject.HasParent("Colorized")){
                        renderer.color = spawnColor * 0.5f ;
                    }
                    renderer.color = Color.white * 0.5f;
                }
                else
                {
                    if (renderer.enabled) renderer.enabled = false;
                }
            }
        }
    }

    
    void SetIndicator()
    {
        if (Application.isPlaying) return;
        if (Indicator != null)
        {
            DestroyImmediate(Indicator);
        }
        GameObject preexisting = GameObject.Find("Indicator");
        if (preexisting != null) DestroyImmediate(preexisting);
        if (selectedPiece != PieceType.none)
        {
            if (RoomManager.pieceGameObjects == null)
            {
                Debug.Log("pieceGameObjects is null");
            }
            else if (RoomManager.pieceGameObjects[selectedPiece] == null)
            {
                Debug.Log(selectedPiece + " was not found in the dictionary");
            }

            Indicator = (GameObject)Instantiate(RoomManager.pieceGameObjects[selectedPiece]);
            
            foreach (var r in Indicator.GetComponentsInChildren<SpriteRenderer>()) r.color *= 0.5f;

            Indicator.name = "Indicator";
            GamePiece piece = Indicator.GetComponent<GamePiece>();
            if (piece != null)
            {
                piece.colorslot = colorslot;
            }
        }
    }
    enum TeleportTool { off, selectSource, selectTarget }
    TeleportTool teleportTool = TeleportTool.off;
    void ProcessTeleportTool()
    {
        //if (Event.current.type != EventType.KeyDown) return;
        if (teleportTool == TeleportTool.off)
        {
            if (Event.current.keyCode == KeyCode.T)
            {
                teleportTool = TeleportTool.selectSource;
                Debug.Log(Event.current);
                RoomManager.roomManager.GetPiecesOfColor(ColorSlot.B);
            }
        }
        else if (teleportTool == TeleportTool.selectSource)
        {
            if (Event.current.keyCode == KeyCode.T)
            {
                teleportTool = TeleportTool.selectTarget;
                //Debug.Log("On again");
            }
        }
        else if (teleportTool == TeleportTool.selectTarget)
        {
            if (Event.current.keyCode == KeyCode.T)
            {
                teleportTool = TeleportTool.off;
                //Debug.Log("On third time");
            }
        }
    }
    void OnUpdate(SceneView sceneView)
    {
		Vector2 screenpos = Event.current.mousePosition;
		screenpos.y = sceneView.camera.pixelHeight - screenpos.y;
		MousePos = sceneView.camera.ScreenPointToRay(screenpos).origin;
        bool withinGrid = MousePos.isWithinGrid();
        if (withinGrid)
        {
            if (Event.current.type == EventType.MouseDown && Event.current.button == 2)
            {
                Active = !Active;
                Event.current.Use(); //keeping this line causes one bug, removing it causes another.
            }
        }
        if (Active)
        {
            UpdateIndicator(withinGrid);
        }
        ProcessTeleportTool();
		if(Active && withinGrid)
        {
	        if (Event.current.type == EventType.layout)
	            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(GetHashCode(), FocusType.Passive));
	        Selection.activeObject = null;
            HandleNumKeys();
			RightClick = isRightPressed(RightClick);
			LeftClick = isLeftPressed(LeftClick);
            Vector2 ScrollVect = getScroll();

            int scroll = Math.Sign(ScrollVect.y);
            if (scroll != 0)
            {
                int nextPiece = (int)selectedPiece + scroll;
                if (nextPiece >= 0 && nextPiece < Enum.GetValues(typeof(PieceType)).Length)
                {
                    selectedPiece = (PieceType)nextPiece;
                    SetIndicator();
                }
            }
            if (selectedPiece == PieceType.wall || selectedPiece == PieceType.door)
            {
				Side side;
                Wall.Orientation or;
				Vector2 target = Utils.WorldToWallPos(MousePos, out side, out or);
                if (Indicator)
                {
                    Indicator.transform.position = target;
                    Indicator.GetComponent<Wall>().orientation = or;
                    if (selectedPiece == PieceType.wall)
                    {
                    }
                    else if (selectedPiece == PieceType.door)
                    {
                        Indicator.GetComponent<Door>().colorslot = colorslot;
                    }
                }
                if (LeftDown())
                {
                    if (selectedPiece == PieceType.wall)
                    {
                        SpawnWall(target, or, side);
                    }
                    else if (selectedPiece == PieceType.door)
                    {
                        SpawnDoor(target, or, side, colorslot);
                    }
					sceneView.Update();
					sceneView.Repaint();
                }
                else if (RightDown())
                {
					RoomManager.roomManager.RemoveWall(MousePos);
                    sceneView.Update();
                    sceneView.Repaint();
                }
                
            }
            else if (selectedPiece == PieceType.player)
            {
                Cell target = Cell.GetFromWorldPos(MousePos);
                if (target != null)
                {
                    Indicator.transform.position = target.WorldPos();
                    if (LeftDown())
                    {
                        //SpawnPiece(selectedPiece, target);
                        SpawnPlayer(target);
                        sceneView.Update();
                        sceneView.Repaint();
                    }
                    else if (RightDown())
                    {
                        RoomManager.roomManager.RemoveTopPiece(target);
                        sceneView.Update();
                        sceneView.Repaint();
                    }
                }
            }
            else if (selectedPiece != PieceType.none)//spawn any other piece (other than wall)
            {
				Cell target = Cell.GetFromWorldPos(MousePos);
				if (target!=null) {
					if (Indicator != null) Indicator.transform.position = target.WorldPos();
                    if (LeftDown())
                    {
					    SpawnPiece(selectedPiece, target);
						sceneView.Update();
						sceneView.Repaint();
					}
					else if (RightDown())
					{
                        RoomManager.roomManager.RemoveTopPiece(target);
                        sceneView.Update();
                        sceneView.Repaint();
					}
				}
            }
			Event.current.Use();
		}
    }

    int? colorSlotCount = null;

    public void HandleNumKeys()
    {
        if (Event.current.type == EventType.KeyDown)
        {
            if (colorSlotCount == null)
            {
                colorSlotCount = Enum.GetValues(typeof(ColorSlot)).Length;
            }
            ColorSlot newColorSlot = colorslot;
            int code = (int)Event.current.keyCode;
            int alpha = code - (int)KeyCode.Alpha0;
            int keypad = code - (int)KeyCode.Keypad0;
            if (alpha >= 0 && alpha < colorSlotCount)
                newColorSlot = (ColorSlot)(alpha);
            else if (keypad >= 0 && keypad < colorSlotCount)
                newColorSlot = (ColorSlot)(keypad);

            if (colorslot != newColorSlot)
            {
                colorslot = newColorSlot;
                spawnColor = Author.GetColorSlot(colorslot);
            }

            UpdateIndicator(true);
        }
    }
    public void SpawnPiece(PieceType piece, Cell target)
    {
        if (target.IsSolidlyOccupied()) return;
        GameObject parent = GetPieceParent(piece);
        GameObject obj = (GameObject)PrefabUtility.InstantiatePrefab(RoomManager.pieceGameObjects[selectedPiece]);
        obj.transform.position = target.WorldPos();
        obj.transform.parent = parent.transform;
        RoomManager.roomManager.AddPiece(obj, piece, colorslot);

    }
    public void SpawnPlayer(Cell target)
    {
        if (RoomManager.roomManager.player == null)
        {
            GameObject obj = (GameObject)PrefabUtility.InstantiatePrefab(RoomManager.pieceGameObjects[PieceType.player]);
            obj.transform.position = target.WorldPos();
            RoomManager.roomManager.AddPiece(obj, PieceType.player, ColorSlot.none);
        }
        else
        {
            RoomManager.roomManager.player.TeleportTo(target);
        }
    }
    public void SpawnWall(Vector3 target, Wall.Orientation orient, Side side)
    {
        GameObject wallobj = (GameObject)PrefabUtility.InstantiatePrefab(RoomManager.pieceGameObjects[PieceType.wall]);
        wallobj.transform.position = target;
        wallobj.transform.parent = GetPieceParent(PieceType.wall).transform;
        Wall wall = null;
        wall = wallobj.GetComponent<Wall>();
		if(wall == null) throw new WTFException();
		wall.orientation = orient;
        RoomManager.roomManager.AddWall(wall);
    }
    public void SpawnDoor(Vector3 target, Wall.Orientation orient, Side side, ColorSlot colorslot)
    {
        GameObject doorobj = (GameObject)PrefabUtility.InstantiatePrefab(RoomManager.pieceGameObjects[PieceType.door]);
        doorobj.transform.position = target;
        doorobj.transform.parent = GetPieceParent(PieceType.door).transform;
        Door door = null;
        door = doorobj.GetComponent<Door>();
        if (door == null) throw new WTFException();
        door.orientation = orient;
        door.SetColorSlot(colorslot);
        RoomManager.roomManager.AddWall(door);
    }
    public GameObject GetPieceParent(PieceType piece)
    {
        if (!RoomManager.pieceParents.ContainsKey(piece) || RoomManager.pieceParents[piece] == null)
        {
            GameObject preexisting = GameObject.Find(piece.ToString());
            if (preexisting != null && preexisting.transform.parent == null)
            {
                RoomManager.pieceParents[piece] = preexisting;
            }
            else
            {
                RoomManager.pieceParents[piece] = new GameObject();
                RoomManager.pieceParents[piece].name = piece.ToString();
            }
        }
        return RoomManager.pieceParents[piece];
    }


    bool isLeftPressed(bool prevValue)
    {
		if(LeftDown()) return true;
        if(Event.current.type == EventType.MouseUp && Event.current.button == 0) return false;
        return prevValue;
    }
    bool isRightPressed(bool prevValue)
    {
		if (RightDown()) return true;
        if (Event.current.type == EventType.MouseUp && Event.current.button == 1) return false;
        return prevValue;
    }
	bool RightDown(){
		return (Event.current.type == EventType.MouseDown && Event.current.button == 1);
	}
	bool LeftDown(){
		return (Event.current.type == EventType.MouseDown && Event.current.button == 0);
	}

    Vector2 getScroll() {return Event.current.type == EventType.ScrollWheel ? Event.current.delta :  new Vector2();}
    void OnGUI()
    {
        //EditorGUILayout.Vector2Field("MousePos", MousePos);
        Active =  (EditorGUILayout.Toggle("Active", Active));// && WallPrefab != null);
        //WallPrefab = (GameObject)EditorGUILayout.ObjectField("WallPrefab", WallPrefab, typeof(GameObject), false);

        PieceType newpiece = (PieceType)EditorGUILayout.EnumPopup("Select Piece:", selectedPiece);
        if (selectedPiece != newpiece)
        {
            selectedPiece = newpiece;
            SetIndicator();
        }
        ColorSlot newColor = (ColorSlot)EditorGUILayout.EnumPopup("Select Color:", colorslot);
        if (newColor != colorslot)
        {
            colorslot = newColor;
            spawnColor = Author.GetColorSlot(colorslot);
        }
        EditorGUILayout.ColorField("Color Preview:", spawnColor);
    }
    void OnInspectorUpdate()
    {
        Repaint();
    }
}
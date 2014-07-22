using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System;
using System.Linq;

public class PuzzleMaker : EditorWindow
{
    [MenuItem("BlockIt/PuzzleMaker")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(PuzzleMaker));
    }


    public int selectedIndex = 1;
    ColorSlot colorslot = ColorSlot.None;
    Color spawnColor = Color.white;
    Vector2 MousePos;
    bool LeftClick;
    bool RightClick;
	bool Active;
    GameObject Indicator;
    public static List<Type> PieceTypeList;
    public Type selectedPiece { get { return PieceTypeList[selectedIndex]; } }
    static PuzzleMaker()
    {
        PieceTypeList = new List<Type>();
        PieceTypeList.Add(null);
        PieceTypeList.Add(typeof(Wall));
        
        foreach (Type t in typeof(GamePiece).Assembly.GetTypes().Where(type => type.IsSubclassOf(typeof(GamePiece)) && !type.IsAbstract))
        {
            PieceTypeList.Add(t);
        }
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
                    if (selectedPiece != typeof(Wall)
                        && renderer.gameObject.HasParent("Colorized"))
                    {
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
        if (selectedIndex != 0)
        {

            Indicator = (GameObject)PrefabUtility.InstantiatePrefab(GamePiece.GetPrefab(selectedPiece));
            
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
                RoomManager.roomManager.GetPiecesOfColor(ColorSlot.Blue);
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
                int nextPiece = (int)selectedIndex + scroll;
                if (nextPiece >= 0 && nextPiece < PieceTypeList.Count)
                {
                    selectedIndex = nextPiece;
                    SetIndicator();
                }
            }
            if (selectedPiece == typeof(Wall))
            {
				Side side;
                Orientation or;
				Vector2 target = Utils.WorldToWallPos(MousePos, out side, out or);
                if (Indicator)
                {
                    Indicator.transform.position = target;
                    Wall wall = Indicator.GetComponent<Wall>();
                    wall.orientation = or;

                    if (wall.isDoor)
                    {
                        Indicator.GetComponent<Wall>().colorslot = colorslot;
                    }
                }
                if (LeftDown())
                {
                    SpawnWall(target, or, side, colorslot);
                    
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
            else if (selectedPiece == typeof(Player))
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
            else if (selectedIndex != 0)//spawn any other piece (other than wall)
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
                spawnColor = MetaData.GetColorSlot(colorslot);
            }

            UpdateIndicator(true);
        }
    }
    public void SpawnPiece(Type piece, Cell target)
    {
        if (target.IsSolidlyOccupied()) return;
        GameObject parent = GetPieceParent(piece);
        GameObject obj = (GameObject)PrefabUtility.InstantiatePrefab(GamePiece.GetPrefab(selectedPiece));
        obj.transform.position = target.WorldPos();
        obj.transform.parent = parent.transform;
        RoomManager.roomManager.AddPiece(obj, piece, colorslot);

    }
    public void SpawnPlayer(Cell target)
    {
        if (RoomManager.roomManager.player == null)
        {
            GameObject obj = (GameObject)PrefabUtility.InstantiatePrefab(GamePiece.GetPrefab(typeof(Player)));
            obj.transform.position = target.WorldPos();
            RoomManager.roomManager.AddPiece(obj, typeof(Player), ColorSlot.None);
        }
        else
        {
            RoomManager.roomManager.player.TeleportTo(target);
        }
    }
    public void SpawnWall(Vector3 target, Orientation orient, Side side, ColorSlot colorslot)
    {
        GameObject wallobj = (GameObject)PrefabUtility.InstantiatePrefab(GamePiece.GetPrefab(typeof(Wall)));
        wallobj.transform.position = target;
        wallobj.transform.rotation = orient == Orientation.Vertical ? Quaternion.identity : Quaternion.Euler(0, 0, 90);
        wallobj.transform.parent = GetPieceParent(typeof(Wall)).transform;
        Wall wall = null;
        wall = wallobj.GetComponent<Wall>();
		if(wall == null) throw new WTFException();
		wall.orientation = orient;
        wall.SetColorSlot(colorslot);
        RoomManager.roomManager.AddWall(wall);
    }

    public static GameObject GetPieceParent(Type piece)
    {
        if (!RoomManager.pieceParents.ContainsKey(piece) || RoomManager.pieceParents[piece] == null)
        {
            GameObject preexisting = GameObject.Find(piece.ToString().UppercaseFirst() + " Group");
            if (preexisting != null && preexisting.transform.parent == RoomManager.masterParent.transform)
            {
                RoomManager.pieceParents[piece] = preexisting;
            }
            else
            {
                RoomManager.pieceParents[piece] = new GameObject();
                RoomManager.pieceParents[piece].name = piece.ToString().UppercaseFirst() + " Group";
                RoomManager.pieceParents[piece].transform.parent = RoomManager.masterParent.transform;
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

        int newpiece = EditorGUILayout.Popup("Select Piece:", selectedIndex, PieceTypeList.Select(t=>t!=null?t.Name:"None").ToArray());
        if (selectedIndex != newpiece)
        {
            selectedIndex = newpiece;
            SetIndicator();
        }
        ColorSlot newColor = (ColorSlot)EditorGUILayout.EnumPopup("Select Color:", colorslot);
        if (newColor != colorslot)
        {
            colorslot = newColor;
            spawnColor = MetaData.GetColorSlot(colorslot);
        }
        EditorGUILayout.ColorField("Color Preview:", spawnColor);
    }
    void OnInspectorUpdate()
    {
        Repaint();
    }
}
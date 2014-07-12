using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System;

public class PuzzleMaker : EditorWindow
{
    public PieceType selectedPiece = PieceType.wall;
    
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
    
    void Initialize()
    {
        
        if (Indicator == null)
        {
            SetIndicator();
        }
        //if (WallParent == null)
        //{
        //    WallParent = new GameObject();
        //    WallParent.name = "Walls";
        //}
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
            if (RoomManager.pieceGameObjects == null || RoomManager.pieceGameObjects[selectedPiece] == null)
                Debug.Log("Why?");
            Indicator = (GameObject)Instantiate(RoomManager.pieceGameObjects[selectedPiece]);
            Indicator.GetComponent<SpriteRenderer>().color *= 0.5f;
            Indicator.name = "Indicator";
        }
    }
    void OnUpdate(SceneView sceneView)
    {
		

		Vector2 screenpos = Event.current.mousePosition;
		screenpos.y = sceneView.camera.pixelHeight - screenpos.y;
		MousePos = sceneView.camera.ScreenPointToRay(screenpos).origin;
        if (MousePos.isWithinGrid() && Event.current.type == EventType.MouseDown && Event.current.button == 2)
        {
            Active = !Active; Event.current.Use();
        }
		if(Active && MousePos.isWithinGrid())
        {
            Initialize();
            
	        if (Event.current.type == EventType.layout)
	            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(GetHashCode(), FocusType.Passive));
	        Selection.activeObject = null;
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


            if (selectedPiece == PieceType.wall )
            {
				Side side;
                Wall.Orientation or;
				Vector2 target = Utils.WorldToWallPos(MousePos, out side, out or);
                Indicator.transform.position = target;
                Indicator.GetComponent<Wall>().orientation = or;
                if (LeftDown())
                {
                    SpawnWall(target, or, side);
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
            else if (selectedPiece == PieceType.none)
            {
                
            }
			Event.current.Use();
		}
    }
    public void SpawnPiece(PieceType piece, Cell target)
    {
        if (target.IsSolidlyOccupied()) return;
        GameObject parent = GetPieceParent(piece);
        GameObject obj = (GameObject)Instantiate(RoomManager.pieceGameObjects[selectedPiece], target.WorldPos(), Quaternion.identity);
        obj.transform.parent = parent.transform;
        RoomManager.roomManager.AddPiece(obj, piece);

    }
    public void SpawnPlayer(Cell target)
    {
        if (RoomManager.roomManager.player == null)
        {
            GameObject obj = (GameObject)Instantiate(RoomManager.pieceGameObjects[PieceType.player], target.WorldPos(), Quaternion.identity);
            RoomManager.roomManager.AddPiece(obj, PieceType.player);
        }
        else
        {
            RoomManager.roomManager.player.TeleportTo(target);
        }
        

    }
    public void SpawnWall(Vector3 target, Wall.Orientation orient, Side side)
    {
        GameObject wallobj = (GameObject)Instantiate(RoomManager.pieceGameObjects[PieceType.wall], target, Quaternion.identity);
		wallobj.transform.parent = GetPieceParent(PieceType.wall).transform;
		Wall wall = wallobj.GetComponent<Wall>();
		if(wall == null) throw new WTFException();
		wall.orientation = orient;
        //wall.Update();
        RoomManager.roomManager.AddWall(wall);
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
    }
    void OnInspectorUpdate()
    {
        Repaint();
    }

}
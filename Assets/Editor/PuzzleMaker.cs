using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;

public class PuzzleMaker : EditorWindow
{
    public PieceType selectedPiece = PieceType.wall;
    public Dictionary<PieceType, GameObject> pieceGameObjects;
    public Dictionary<PieceType, GameObject> pieceParents;
    Vector2 MousePos;
    bool LeftClick;
    bool RightClick;
	bool Active;
    GameObject Indicator;
    GameObject WallParent;
    [MenuItem("Window/PuzzleMaker")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(PuzzleMaker));
    }
    void OnEnable()
    {        
        SceneView.onSceneGUIDelegate += OnUpdate;
    }
	void OnDisable()
    {
		SceneView.onSceneGUIDelegate -= OnUpdate;
	}
    void Initialize()
    {
        if (pieceGameObjects == null)
        {
            pieceGameObjects = new Dictionary<PieceType, GameObject>()
            {
                { PieceType.antitrap, Resources.Load<GameObject>("Prefabs/anti-trap")},
                { PieceType.button, Resources.Load<GameObject>("Prefabs/button")},
                { PieceType.end, Resources.Load<GameObject>("Prefabs/end")},
                { PieceType.key, Resources.Load<GameObject>("Prefabs/key")},
                { PieceType.keyhole, Resources.Load<GameObject>("Prefabs/Keyhole")},
                { PieceType.player, Resources.Load<GameObject>("Prefabs/player")},
                { PieceType.start, Resources.Load<GameObject>("Prefabs/start")},
                { PieceType.teleport, Resources.Load<GameObject>("Prefabs/teleport")},
                { PieceType.trap, Resources.Load<GameObject>("Prefabs/trap")},
                { PieceType.wall, Resources.Load<GameObject>("Prefabs/Wall")},
            };
            pieceParents = new Dictionary<PieceType, GameObject>();
        }
        if (Indicator == null)
        {
            SetIndicator();
        }
        if (WallParent == null)
        {
            WallParent = new GameObject();
            WallParent.name = "Walls";
        }
    }

    
    void SetIndicator()
    {
        if (Indicator != null)
        {
            DestroyImmediate(Indicator);
        }
        if (selectedPiece != PieceType.none)
        {
            Indicator = (GameObject)Instantiate(pieceGameObjects[selectedPiece]);
            Indicator.GetComponent<SpriteRenderer>().color *= 0.5f;
            Indicator.name = "Indicator";
        }
    }
    void OnUpdate(SceneView sceneView)
    {
		
		Vector2 screenpos = Event.current.mousePosition;
		screenpos.y = sceneView.camera.pixelHeight - screenpos.y;
		MousePos = sceneView.camera.ScreenPointToRay(screenpos).origin;

		if(Active && MousePos.isWithinGrid())
        {
            Initialize();
            
	        if (Event.current.type == EventType.layout)
	            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(GetHashCode(), FocusType.Passive));
	        Selection.activeObject = null;
			RightClick = isRightPressed(RightClick);
			LeftClick = isLeftPressed(LeftClick);
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
                else if (Event.current.type == EventType.MouseDown && Event.current.button == 2)
                {
                    RoomManager.roomManager.PrintWallAmount();
                }
            }
            else if (selectedPiece != PieceType.none)//spawn any other piece (other than wall)
            {
				Cell target = Cell.GetFromWorldPos(MousePos);
				if (target!=null) {
					Indicator.transform.position = target.WorldPos();
                    if (LeftDown())
                    {
					    SpawnPiece(selectedPiece, target);
						sceneView.Update();
						sceneView.Repaint();
					}
					else if (RightDown())
					{
                        bool destroyChildren = false;
                        RoomManager.roomManager.RemovePiece(target, destroyChildren);
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
        GameObject parent = GetPieceParent(piece);
		GameObject obj = (GameObject)Instantiate(pieceGameObjects[selectedPiece], target.WorldPos(), Quaternion.identity);
        obj.transform.parent = parent.transform;
        RoomManager.roomManager.AddPiece(obj, piece);

    }
    public void SpawnWall(Vector3 target, Wall.Orientation orient, Side side)
    {
        GameObject wallobj = (GameObject)Instantiate(pieceGameObjects[PieceType.wall], target, Quaternion.identity);
		wallobj.transform.parent = GetPieceParent(PieceType.wall).transform;
		Wall wall = wallobj.GetComponent<Wall>();
		if(wall == null) throw new WTFException();
		wall.orientation = orient;
        wall.Update();
        RoomManager.roomManager.AddWall(wall);
    }
    public GameObject GetPieceParent(PieceType piece)
    {
        if (!pieceParents.ContainsKey(piece) || pieceParents[piece] == null)
        {
            pieceParents[piece] = new GameObject();
            pieceParents[piece].name = piece.ToString();
        }
        return pieceParents[piece];
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
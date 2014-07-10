using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
class PuzzleMaker : EditorWindow
{
    public enum pieces
    {
        none,
        antitrap,
        button,
        end,
        key,
        keyhole,
        player,
        start,
        teleport,
        trap,
        wall,
    }
    public pieces selectedPiece = pieces.wall;
    public Dictionary<pieces, GameObject> pieceGameObjects;
    public Dictionary<pieces, GameObject> pieceParents;
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
            pieceGameObjects = new Dictionary<pieces, GameObject>()
            {
                { pieces.antitrap, Resources.Load<GameObject>("Prefabs/anti-trap")},
                { pieces.button, Resources.Load<GameObject>("Prefabs/button")},
                { pieces.end, Resources.Load<GameObject>("Prefabs/end")},
                { pieces.key, Resources.Load<GameObject>("Prefabs/key")},
                { pieces.keyhole, Resources.Load<GameObject>("Prefabs/Keyhole")},
                { pieces.player, Resources.Load<GameObject>("Prefabs/player")},
                { pieces.start, Resources.Load<GameObject>("Prefabs/start")},
                { pieces.teleport, Resources.Load<GameObject>("Prefabs/teleport")},
                { pieces.trap, Resources.Load<GameObject>("Prefabs/trap")},
                { pieces.wall, Resources.Load<GameObject>("Prefabs/Wall")},
            };
            pieceParents = new Dictionary<pieces, GameObject>();
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
        if (selectedPiece != pieces.none)
        {
            Indicator = (GameObject)Instantiate(pieceGameObjects[selectedPiece]);
            Indicator.GetComponent<SpriteRenderer>().color *= 0.5f;
            Indicator.name = "Indicator";
        }
    }
    void OnUpdate(SceneView sceneView)
    {
		if(Active)
        {
            Initialize();
            
	        if (Event.current.type == EventType.layout)
	            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(GetHashCode(), FocusType.Passive));
	        Selection.activeObject = null;
			RightClick = isRightPressed(RightClick);
			LeftClick = isLeftPressed(LeftClick);
			Vector2 screenpos = Event.current.mousePosition;
			screenpos.y = sceneView.camera.pixelHeight - screenpos.y;
			MousePos = sceneView.camera.ScreenPointToRay(screenpos).origin;
            if (selectedPiece == pieces.wall)
            {
                Wall.Orientation or;
                Vector2 target = GetWallTarget(sceneView, out or);
                Indicator.transform.position = target;
                Indicator.GetComponent<Wall>().orientation = or;
                if (LeftDown())
                {
                    SpawnWall(target, or, sceneView);
                }
                else if (RightDown())
                {
                    RoomManager.roomManager.RemoveWall(target, or);
                    sceneView.Update();
                    sceneView.Repaint();
                }
            }
            else if (selectedPiece != pieces.none)//spawn any other piece (other than wall)
            {
                Vector2 target = GetPieceTarget(sceneView);
                Indicator.transform.position = target;
                if (LeftDown())
                {
                    SpawnObject(selectedPiece, target, sceneView);
                }
                else if (RightDown())
                {
                    //RoomManager.roomManager.RemoveWall(target, or);
                    //sceneView.Update();
                    //sceneView.Repaint();
                }
            }
            else if (selectedPiece == pieces.none)
            {
                
            }
			Event.current.Use();
		}
    }
    public void SpawnObject(pieces piece, Vector3 target, SceneView sceneView)
    {
        GameObject parent = GetPieceParent(piece);
        GameObject obj = (GameObject)Instantiate(pieceGameObjects[selectedPiece], target, Quaternion.identity);
        obj.transform.parent = parent.transform;
        //add object to room manager
        //

        sceneView.Update();
        sceneView.Repaint();
    }
    public void SpawnWall(Vector3 target, Wall.Orientation orient, SceneView sceneView)
    {
        GameObject wall = (GameObject)Instantiate(pieceGameObjects[pieces.wall], target, Quaternion.identity);
        wall.transform.parent = GetPieceParent(pieces.wall).transform;
        wall.GetComponent<Wall>().orientation = orient;
        RoomManager.roomManager.AddWall(wall);
        sceneView.Update();
        sceneView.Repaint();
    }
    public GameObject GetPieceParent(pieces piece)
    {
        if (!pieceParents.ContainsKey(piece))
        {
            pieceParents[piece] = new GameObject();
            pieceParents[piece].name = piece.ToString();
        }
        return pieceParents[piece];
    }
    Vector2 GetPieceTarget(SceneView sceneView)
    {
        int blockSize = Wall.blockSize;
        float originX = ((int)Mathf.Floor(MousePos.x / blockSize)) * blockSize;
        float originY = ((int)Mathf.Floor(MousePos.y / blockSize)) * blockSize;
        float middleX = originX + (blockSize / 2f);
        float middleY = originY + (blockSize / 2f);
        return new Vector2(middleX, middleY);
    }
    Vector2 GetWallTarget(SceneView sceneView, out Wall.Orientation orientation)
    {
        int blockSize = Wall.blockSize;
        float originX = ((int)Mathf.Floor(MousePos.x / blockSize)) * blockSize;
        float originY = ((int)Mathf.Floor(MousePos.y / blockSize)) * blockSize;
        float x = MousePos.x - originX;
        float y = MousePos.y - originY;
        Vector3 vect = Vector3.zero;
        if (x > y)
        {
            if (x < blockSize - y)
            {
                //area bottom
                vect.x += blockSize / 2;
                orientation = Wall.Orientation.Horizontal;
            }
            else
            {
                //area right
                vect.x += blockSize;
                vect.y += blockSize / 2;
                orientation = Wall.Orientation.Vertical;
            }
        }
        else
        {
            if (x < blockSize - y)
            {
                //area left
                vect.y += blockSize / 2;
                orientation = Wall.Orientation.Vertical;
            }
            else
            {
                //area top
                vect.x += blockSize / 2;
                vect.y += blockSize;
                orientation = Wall.Orientation.Horizontal;
            }
        }
        vect.x += originX;
        vect.y += originY;
        return vect;
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

        pieces newpiece = (pieces)EditorGUILayout.EnumPopup("Select Piece:", selectedPiece);
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
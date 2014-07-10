using UnityEngine;
using System.Collections;
using UnityEditor;
class PuzzleMaker : EditorWindow
{

    Vector2 MousePos;
    bool LeftClick;
    bool RightClick;
	bool Active;
	GameObject WallPrefab;
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
	void OnDisable(){
		SceneView.onSceneGUIDelegate -= OnUpdate;  
	}
    void OnUpdate(SceneView sceneView)
    {
        

		if(Active){
            if (Indicator == null)
            {
                Indicator = (GameObject)Instantiate(WallPrefab);
                Indicator.GetComponent<SpriteRenderer>().color *= 0.5f;
                Indicator.name = "Indicator";
            }
            if (WallParent == null)
            {
                WallParent = new GameObject();
                WallParent.name = "Walls";
            }
            
	        if (Event.current.type == EventType.layout)
	            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(GetHashCode(), FocusType.Passive));
	        Selection.activeObject = null;
			RightClick = isRightPressed(RightClick);
			LeftClick = isLeftPressed(LeftClick);



			Vector2 screenpos = Event.current.mousePosition;
			screenpos.y = sceneView.camera.pixelHeight - screenpos.y;
			MousePos = sceneView.camera.ScreenPointToRay(screenpos).origin;

            Wall.Orientation or;
            Vector2 target = GetWallTarget(sceneView, out or);

            Indicator.transform.position = target;
            Indicator.GetComponent<Wall>().orientation = or;

			if(LeftDown ()){
                GameObject wall = (GameObject)Instantiate(WallPrefab, target, Quaternion.identity);
                wall.transform.parent = WallParent.transform;
                wall.GetComponent<Wall>().orientation = or;
                RoomManager.roomManager.AddWall(wall);
                sceneView.Update();
                sceneView.Repaint();
            } if (RightDown())
            {
                RoomManager.roomManager.RemoveWall(target, or);
                sceneView.Update();
                sceneView.Repaint();
            }
			Event.current.Use();
		}
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
        Rect r =  EditorGUILayout.BeginHorizontal("Button");
        if (GUI.Button(r, GUIContent.none))
        {
            Debug.Log("DO sometninggasd");
        }
        GUILayout.Label("Make Wall");
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Vector2Field("MousePos", MousePos);
        Active =  (EditorGUILayout.Toggle("Active", Active) && WallPrefab != null);
		
        EditorGUILayout.Toggle("LeftClick", LeftClick);
        EditorGUILayout.Toggle("RightClick", RightClick);
		WallPrefab = (GameObject)EditorGUILayout.ObjectField("WallPrefab",WallPrefab,typeof(GameObject),false);


    }
    void OnInspectorUpdate()
    {
        Repaint();
    }

}
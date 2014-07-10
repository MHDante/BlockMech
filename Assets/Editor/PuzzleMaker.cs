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
	        if (Event.current.type == EventType.layout)
	            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(GetHashCode(), FocusType.Passive));
	        Selection.activeObject = null;
			RightClick = isRightPressed(RightClick);
			LeftClick = isLeftPressed(LeftClick);


			Vector2 screenpos = Event.current.mousePosition;
			screenpos.y = sceneView.camera.pixelHeight - screenpos.y;
			MousePos = sceneView.camera.ScreenPointToRay(screenpos).origin;

			if(LeftDown ()){
				Instantiate(WallPrefab, new Vector3(MousePos.x, MousePos.y, 0), Quaternion.identity);
				sceneView.Update();
				sceneView.Repaint();
				HandleUtility.Repaint();
				SceneView.RepaintAll();



			}
			Event.current.Use();
		}
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
        Active =  EditorGUILayout.Toggle("Active", Active);
		
        EditorGUILayout.Toggle("LeftClick", LeftClick);
        EditorGUILayout.Toggle("RightClick", RightClick);
		WallPrefab = (GameObject)EditorGUILayout.ObjectField("WallPrefab",WallPrefab,typeof(GameObject),false);


    }
    void OnInspectorUpdate()
    {
        Repaint();
    }

}
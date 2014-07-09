using UnityEngine;
using System.Collections;
using UnityEditor;
class PuzzleMaker : EditorWindow
{

    Vector2 MousePos;
    bool LeftClick;
    bool RightClick;
    [MenuItem("Window/PuzzleMaker")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(PuzzleMaker));
    }
    void OnEnable()
    {
        
        SceneView.onSceneGUIDelegate += OnUpdate;   
    }
    void OnUpdate(SceneView sceneView)
    {
        if (Event.current.type == EventType.layout)
            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(GetHashCode(), FocusType.Passive));
        Selection.activeObject = null;
        Event e = Event.current;
        MousePos = e.mousePosition;
        RightClick = isRightDown(RightClick);
        LeftClick = LeftPressed(LeftClick);
        Debug.Log(RightClick);
        Event.current.Use();


    }



    bool LeftPressed(bool prevValue)
    {

        if(Event.current.type == EventType.MouseDown && Event.current.button == 0) return true;
        if(Event.current.type == EventType.MouseUp && Event.current.button == 0) return false;
        return prevValue;
    }
    bool isRightDown(bool prevValue)
    {
        if (Event.current.type == EventType.MouseDown && Event.current.button == 1) return true;
        if (Event.current.type == EventType.MouseUp && Event.current.button == 1) return false;
        return prevValue;
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
        EditorGUILayout.Toggle("LeftClick", LeftClick);
        EditorGUILayout.Toggle("RightClick", RightClick);
        EditorGUILayout.ObjectField(")

    }
    void OnInspectorUpdate()
    {
        Repaint();
    }


}
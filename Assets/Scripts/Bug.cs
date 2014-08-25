using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Bug : MonoBehaviour
{
    public static float margin;
    //debug window stuff
    bool guiInitDone = false;
    Rect rectDebugArea;
    Vector2 scrollPosition;
    int fontHD;

    static string debugCumulativeStr = "";
    static int debugLogLineCount = 0;
    static bool debugStrChanged = false;

    GUIStyle debugStyle;
    GUIStyle statsStyle;
    void Start()
    {
        margin = 0.05f * Screen.width;
    }
    void OnGUI()
    {
        //Color tags in rich text / OnGui : http://answers.unity3d.com/questions/360885/color-tags-in-rich-text-ongui.html
        if (!guiInitDone) { InitGUI(); }

        GUILayout.BeginArea(rectDebugArea);
        scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width(rectDebugArea.width), GUILayout.Height(rectDebugArea.height));
        GUILayout.Box(new GUIContent(debugCumulativeStr, "Mouse Over TextArea"), debugStyle);
        float scrollAmt = Input.GetAxis("Mouse ScrollWheel");
        if (GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
        {  //Event.current.type == EventType.Repaint &&
            scrollPosition = new Vector2(0, scrollPosition.y + scrollAmt * 10);
        }
        GUILayout.EndScrollView();
        GUILayout.EndArea();

        if (debugStrChanged)
        {
            int estimatedStyleMargins = fontHD;
            scrollPosition = new Vector2(0, scrollPosition.y + fontHD + estimatedStyleMargins);
            debugStrChanged = false;
        }
    }


    void InitGUI()
    {

        float debugAreaHeight = 0.20f * Screen.height;
        rectDebugArea = new Rect(margin, Screen.height - debugAreaHeight - margin, Screen.width - margin * 2, debugAreaHeight);
        Debug.Log(rectDebugArea);

        scrollPosition = Vector2.zero;

        float factor = 0.02f;
        if (Application.platform == RuntimePlatform.Android)
        {
            factor = 0.03f;
        }

        fontHD = (int)(factor * Screen.width);

        debugStyle = new GUIStyle(GUI.skin.textArea);
        debugStyle.normal.textColor = Color.white;
        debugStyle.richText = true;
        debugStyle.wordWrap = false;
        debugStyle.fontSize = fontHD;
        debugStyle.alignment = TextAnchor.MiddleLeft;

        guiInitDone = true;
    }

    public static void Log(object obj)
    {
        Log(obj.ToString());
    }
    public static void Log(string str)
    {
        string output = "\n" + ++debugLogLineCount + ") " + str;
        debugCumulativeStr += output;
        debugStrChanged = true; //used to check to push scrollview of debugArea to bottom
        Debug.Log(output.Substring(1)); //sub bc "\n"
    }

}

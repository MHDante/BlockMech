using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

public class Tester : MonoBehaviour {

    // Use this for initialization
    public GUISkin skin;
    
    zEditor editor;
	void Start () {
        float sidebarScreenRatio = 0.1f;
        float buttonWidth = Screen.width * sidebarScreenRatio * 0.7f;
        editor = new zEditor(sidebarScreenRatio, buttonWidth);
	}
    void Update()
    {
        editor.UpdateEditor();
        if (Input.GetKeyDown(KeyCode.A)) ShowKeyboard();
    }

    void OnGUI()
    {
        GUI.skin = skin;
        editor.Draw();
    }
    TouchScreenKeyboard keyboard;
    void ShowKeyboard()
    {
        keyboard = TouchScreenKeyboard.Open("Enter name");
    }

    void Test()
    {
        List<int> list = new List<int> { 1, 2, 3, 4, 5, 6 };
        var grouping = list.GroupBy(x => x % 2 == 0);
        var selected = grouping.Select(x => { int i = x.First(); i++; return i; });
        foreach(var s in selected)
        {
            Debug.Log(s);
        }
    }
    
}

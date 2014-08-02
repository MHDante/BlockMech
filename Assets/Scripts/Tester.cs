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

        if (Input.GetKeyDown(KeyCode.B))
        {
        }
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
    
}

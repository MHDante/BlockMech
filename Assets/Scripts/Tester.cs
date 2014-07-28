using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

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
        //if (Input.GetKeyDown(KeyCode.P)) pieceLayout.Show();
        //if (Input.GetKeyDown(KeyCode.O)) pieceLayout.Hide();
    }

    void OnGUI()
    {
        GUI.skin = skin;
        editor.Draw();
    }
}

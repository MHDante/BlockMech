using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using OrbItUtils;

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
    }
    void OnGUI()
    {
        GUI.skin = skin;
        editor.Draw();
    }
    
    
}

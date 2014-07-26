using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class Tester : MonoBehaviour {

    // Use this for initialization
    zGridLayout layout;
	void Start () {
        List<zButton> zeButtons = new List<zButton>();
        foreach (Type t in RoomManager.PieceTypeList)
        {
                zeButtons.Add(new zButton(t)); 
        }
        layout = new zGridLayout(new Rect(100, 100, Screen.width * .3f, Screen.height * .2f), zeButtons, false);
	}
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.O)) layout.Show();
        if (Input.GetKeyDown(KeyCode.P)) layout.Hide();
        
    }
    void OnGUI()
    {
        layout.Draw();
    }
}

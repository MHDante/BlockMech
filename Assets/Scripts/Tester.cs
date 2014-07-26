using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class Tester : MonoBehaviour {

    // Use this for initialization
    zGridLayout<zButton> layout;
	void Start () {
        List<zButton> zeButtons = new List<zButton>();
        foreach (Type t in RoomManager.PieceTypeList)
        {
            zeButtons.Add(new zButton(t));
        }
        layout = new zGridLayout<zButton>(new Rect(100, 100, Screen.width * .6f, Screen.height * .5f), zeButtons, false);
	}

    void OnGUI()
    {
        layout.Draw();
    }
}

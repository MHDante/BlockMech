using UnityEngine;
using System.Collections;
using System;

[ExecuteInEditMode]
public class Player : GamePiece {


    public override bool isSolid { get; set; }
    public override bool isPushable { get; set; }

	// Use this for initialization
	void Start () {
        //Debug.Log(RoomManager.roomManager != null);
        GameObject g = this.gameObject;
        RoomManager.roomManager.AddPiece(g, piecetype);
	}
	
	// Update is called once per frame
	void Update () {
    }
}

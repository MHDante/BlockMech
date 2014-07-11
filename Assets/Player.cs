using UnityEngine;
using System.Collections;
using System;


public class Player : GamePiece {


    public override bool isSolid { get; set; }
    public override bool isPushable { get; set; }

	public  override void Start () {
        base.Start();
        RoomManager.roomManager.player = this;
        //GameObject g = this.gameObject;
        //RoomManager.roomManager.AddPiece(g, piecetype);
	}
	public  override void Update () {
        base.Update();

        if (Input.GetKey(KeyCode.UpArrow)) { moveTo(Side.top); }
        if (Input.GetKey(KeyCode.DownArrow)) { moveTo(Side.bottom); }
        if (Input.GetKey(KeyCode.LeftArrow)) { moveTo(Side.left); }
        if (Input.GetKey(KeyCode.RightArrow)) { moveTo(Side.right); }

    }

    void OnSwipeUp(){ moveTo(Side.top); }
    void OnSwipeDown() { moveTo(Side.bottom); }
    void OnSwipeLeft() { moveTo(Side.left); }
    void OnSwipeRight() { moveTo(Side.right); }
}

using UnityEngine;
using System.Collections;
using System;


public class Player : GamePiece {


    public override bool isSolid { get; set; }
    public override bool isPushable { get; set; }

    public int Strength = 1;


	public  override void Start () {
        base.Start();
        RoomManager.roomManager.player = this;
        //GameObject g = this.gameObject;
        //RoomManager.roomManager.AddPiece(g, piecetype);
	}
    
	public  override void Update () {
        base.Update();
		if (Input.GetKey(KeyCode.UpArrow)) { TryMove(Side.top); }
		if (Input.GetKey(KeyCode.DownArrow)) { TryMove(Side.bottom); }
		if (Input.GetKey(KeyCode.LeftArrow)) { TryMove(Side.left); }
		if (Input.GetKey(KeyCode.RightArrow)) { TryMove(Side.right); }

    }

	public bool TryMove(Side s){
		if(isMoving) return false;
		if(!moveTo(s)){
			Cell target = cell.getNeighbour(s);
            if (target == null) return false;
			GamePiece obstructor = target.gamePiece;
            if (obstructor != null)
            {
                if (obstructor.pushFrom(Utils.opposite(s),3))
                {
                    obstructor.Detatch();
                    moveTo(s);
                }


            }
                
			return false;//Hit a wall or something
		}
		return true;
	}

	void OnSwipeUp(){ TryMove(Side.top); }
	void OnSwipeDown() { TryMove(Side.bottom); }
	void OnSwipeLeft() { TryMove(Side.left); }
	void OnSwipeRight() { TryMove(Side.right); }
}

using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;


public class Player : GamePiece {


    public override bool isSolid { get; set; }
    public override bool isPushable { get; set; }

    public int Strength = 1;

    public static int steps = 0;
    public static int restarts = 0;
    public List<Key> keys = new List<Key>();

	public override void Start () {
        base.Start();
        RoomManager.roomManager.player = this;
        //GameObject g = this.gameObject;
        //RoomManager.roomManager.AddPiece(g, piecetype);
	}
    
	public override void Update () {
        base.Update();
		if (Input.GetKey(KeyCode.UpArrow)) { TryMove(Side.top); }
		if (Input.GetKey(KeyCode.DownArrow)) { TryMove(Side.bottom); }
		if (Input.GetKey(KeyCode.LeftArrow)) { TryMove(Side.left); }
		if (Input.GetKey(KeyCode.RightArrow)) { TryMove(Side.right); }

        if (Input.GetKeyDown(KeyCode.R))
        {
            if (GameManager.instance != null)
                GameManager.instance.totalRestarts++;
            Application.LoadLevel(Application.loadedLevel);
        }

    }

	public bool TryMove(Side s){
		if(isMoving) return false;
		if(!moveTo(s)){
			Cell target = cell.getNeighbour(s);
            if (target == null) return false;
			GamePiece obstructor = target.firstSolid();
            steps++;
            if (obstructor != null)
            {
                if (obstructor is Keyhole)
                {
                    bool tryopen = ((Keyhole)obstructor).TryOpen(this);
                    if (tryopen) return false; //...
                }
                if (obstructor.pushFrom(s.opposite(),Strength))
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

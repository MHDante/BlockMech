using UnityEngine;
using System.Collections;
using System;

public class Player : GamePiece {

    int moveCounter = 0;
    int distance = 4;
    float timer;
    float moveDelayTime = 0.2f;

    public int gridPosX { get { if (cell != null) return cell.x; return -1; } }
    public int gridPosY { get { if (cell != null) return cell.y; return -1; } }

    public override bool isSolid { get; set; }
    public override bool isPushable { get; set; }
	public override bool moveTo(Side side) { throw new NotImplementedException(); }

	// Use this for initialization
	void Start () {
        GameObject start = GameObject.FindGameObjectWithTag("Start");
        if (start != null) {
            transform.position = start.transform.position;
        }
        else
        {
            Debug.Log("Player requries a start prefab to exist.");
        }
        timer = Time.time;
	}
	
	// Update is called once per frame
	void Update () {
        float x, y;
        x = Input.GetAxis("Horizontal");
        y = Input.GetAxis("Vertical");



        //float deltaTick = Time.time - timer;
        //if (deltaTick > moveDelayTime)
        //{
        //    timer += deltaTick;
        //}
        //if (x != 0 || y != 0)
        //{
        //    transform.position = new Vector3( 
        //        transform.position.x + x * distance, 
        //        transform.position.y + y * distance, 
        //        transform.position.z
        //     );
        //}
    }
}

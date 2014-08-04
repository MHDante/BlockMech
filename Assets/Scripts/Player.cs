using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using OrbItUtils;


public class Player : GamePiece {


    public override bool isSolid { get; set; }
    public override bool isPushable { get; set; }

    public int Strength = 1;

    public List<Key> keys = new List<Key>();





    public class Stats
    {
        public int steps { get; set; }
        public int restarts { get; set; }

        bool timing;
        private DateTime timeStart;
        private DateTime timeEnd;

        private TimeSpan _time;
        public TimeSpan time
        {
            get
            {
                if (timing)
                    return DateTime.Now - timeStart;
                else
                    return _time;
            }
        }


        public Stats()
        {
            steps = 0;
            restarts = 0;
            timeStart = DateTime.Now;
            timing = true;

        }

        public void Stop()
        {
            if (timing)
            {
                timing = false;
                timeEnd = DateTime.Now;
                _time = timeEnd - timeStart;
            }
        }


        public string TimeFormatted()
        {

            //mandatory 3 digits of milliseconds
            //mandatory period between seconds . milliseconds
            //mandatory 1 digit of seconds, optional second.    >> {ref:#0}
            //nicely round everything higher, optionally showing colons as needed

            string output;
           

            if ( (int)time.TotalHours > 0 )
            {
                output = String.Format("{0:#}:{1:00}:{2:00}.{3:000}", time.Hours, time.Minutes, time.Seconds, time.Milliseconds);
            }

            else if ( (int)time.TotalMinutes > 0 ) 
            {
                output = String.Format("{0:#}:{1:00}.{2:000}", time.Minutes, time.Seconds, time.Milliseconds);

            }
            else if ( (int)time.TotalMilliseconds > 0 ) 
            {
                output = String.Format("{0:0}.{1:000}", time.Seconds, time.Milliseconds);

            }

            else
            {
                output = time.ToString(); // fuck it, stop trying format
            }


            return output;

        }
    }

    public static Stats stats;




	public override void Start () {
        base.Start();
        stats = new Stats();
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
        if (!moveTo(s))
        {
            Wall obsWall = cell.getWall(s);
            //cell.getWall(s).transform.rotation.eulerAngles = s==Side.

            Cell target = cell.getNeighbour(s);
            if (target == null)
            {
                if (obsWall != null) obsWall.BendFrom(s);
                return false;
            }
            GamePiece obstructor = target.firstSolid();

            if (obstructor != null)
            {
                if (obstructor is Keyhole)
                {
                    bool tryopen = ((Keyhole)obstructor).TryOpen(this);
                    if (tryopen) return false; //...
                }
                if (obstructor.pushFrom(s.opposite(), Strength))
                {
                    if (obsWall != null) obsWall.BendFrom(s);
                    obstructor.Detatch();
                    moveTo(s);
                }

            }
            else
            {
                if (obsWall != null) obsWall.BendFrom(s);
            }

            return false;//Hit a wall or something
        }
        else 
        {
            //ORIGINAL CODE
            //steps++;
            //Debug.Log(steps);

            //NEW CODE
            stats.steps++;
            //Debug.Log(stats);
            return true;
        }
		
    }

	void OnSwipeUp(){ TryMove(Side.top); }
	void OnSwipeDown() { TryMove(Side.bottom); }
	void OnSwipeLeft() { TryMove(Side.left); }
	void OnSwipeRight() { TryMove(Side.right); }
}

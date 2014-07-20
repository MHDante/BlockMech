using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;


public class Player : GamePiece {


    public override bool isSolid { get; set; }
    public override bool isPushable { get; set; }

    public int Strength = 1;

    public List<Key> keys = new List<Key>();





    public class StatsV4
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


        public StatsV4()
        {
            steps = 0;
            restarts = 0;
            timeStart = DateTime.Now;
            timing = true;

        }

        void Stop()
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

    public static StatsV4 stats;

    /*
    public class StatsV3
    {
        public int steps { get; set; }
        public int restarts { get; set; }

        bool timing;
        private float timeStart;
        private float timeEnd;

        private float _time;
        public float time { get {
            if (timing)
                return Time.time - timeStart;
            else
                return _time;
            } 
        }


        public StatsV3()
        {
            steps = 0;
            restarts = 0;
            timeStart = Time.time;
            timing = true;

        }

        void Stop(){
            if (timing) 
            {
                timing = false;
                timeEnd = Time.time;
                _time = timeEnd - timeStart;
            }
        }

        public override string ToString() 
        {
            return "Steps: " + steps + ". Time: " + time;
        }
    } 

    public static StatsV3 stats;
    */

    //IAN: i considered using static fields, looked up that it's pointless since identical to classes, then gave up thinking about it. :'(

    /*
    public struct StatsV2
    {
        private int _steps;
        private int _restarts;
        private float _timeUsed;
        private float _timeStart;
        private float _timeEnd; 
        public int steps { get { return _steps; } set { if (value > 0) _steps = value; } }
        public int restarts { get { return _restarts; } set { if (value > 0) _restarts = value; } }
        public float timeUsed { get {
            //HA! I put a SETTER in my GETTER! 
            //semantically i have my doubts this is really readable to others... :'(
            if (_timeEnd == null)
            {
                _timeEnd = Time.time;
                _timeUsed = _timeEnd - _timeStart;
            } 
            return _timeUsed; 
        } }

        public void init() 
        {
            _steps = 0;
            _restarts = 0;
            _timeStart = Time.time;
        }
        
    }

    StatsV2 stats = new StatsV2();
    */



    //IAN: as much as i want to abstract away this data, i don't think a class is worth the overhead anymore.

    /*
    public class StatsV1 
    {
        public static int steps;
        public static int restarts;
        public static float time;
        private static float startTime;

        StatsV1() 
        {
            steps = 0;
            restarts = 0;
            startTime = Time.time;

        } 
        // I TRIED WRITING A GET SET HERE AND IT FAILED. I SHOULD TRY HARDER. MAYBE GUIDE ME IN ANALYSIS HERE?


    }
     */


	public override void Start () {
        base.Start();
        stats = new StatsV4();
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
            Cell target = cell.getNeighbour(s);
            if (target == null) return false;
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
                    obstructor.Detatch();
                    moveTo(s);
                }


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

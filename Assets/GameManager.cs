using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

    private static GameManager _instance;

    enum State { Splash, ChoosingWorld, ChoosingLevel, Playing }
    State currentState = State.ChoosingWorld;

    private GUISkin MenuStyle;

    public static GameManager instance {
        get {
            if (_instance == null)
            {
                _instance = GameObject.Find("GameManager").GetComponent<GameManager>();
                DontDestroyOnLoad(_instance.gameObject);
            }
            return _instance;
        }
    }


    void Awake(){

        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(_instance);

            //main game manager logic goes here

            MenuStyle = Resources.Load<GUISkin>("OurSkin");
           


        }
        else 
        {
            //definitely a unity specific issue
            if (this != _instance)
                Destroy(this.gameObject);
        }


    }

    void OnGUI(){

        GUI.skin = MenuStyle;

        //Vector2 padding = new Vector2(50f, 50f);
        float paddingX = 50f;
        float paddingY = 50f;
        float heightCounter = paddingX;
        float widthCounter = paddingY;

        Rect rectLabelBoxit = new Rect(paddingX, paddingY, paddingX + 100, paddingY + 30);

        GUI.Label( rectLabelBoxit, "BLOCK-IT");

        GUI.Box(new Rect(paddingX, paddingY + rectLabelBoxit.height, Screen.width - paddingX * 2, Screen.height - paddingY * 2 - rectLabelBoxit.height), "");


    }
	
}


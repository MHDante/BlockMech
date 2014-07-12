using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

    private static GameManager _instance;

    enum State { Splash, ChoosingWorld, ChoosingLevel, Playing }
    State currentState = State.ChoosingWorld;

    private GUISkin OurSkin;

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

            OurSkin = Resources.Load<GUISkin>("OurSkin");
           


        }
        else 
        {
            //definitely a unity specific issue
            if (this != _instance)
                Destroy(this.gameObject);
        }


    }

    void OnGUI(){

        //target resolutions have screen height on laptop screen in unity editor: 180 px smallest, 320 px largest
        //font height is assumed to refer to the entire height.
        //so the following prototypes with a target of 24 font height in 180 screen height test case.

        float minScreenHeight = 180f;
        float targetFontHeight = 24f;
        float targetFontHeightPercentage = targetFontHeight / minScreenHeight;
        float actualFontHeight = targetFontHeightPercentage * Screen.height;
        Debug.Log(actualFontHeight + " " + (int)actualFontHeight);

        float fontHeight;
        

        GUI.skin = OurSkin;
        GUIStyle myLabelStyle= new GUIStyle(GUI.skin.label);
        //Debug.Log(myLabelStyle.fontSize);
        myLabelStyle.fontSize = (int)actualFontHeight;
        GUI.skin.label = myLabelStyle;


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


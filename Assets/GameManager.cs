using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

    private static GameManager _instance;

    enum State { Splash, ChoosingWorld, ChoosingLevel, Playing }
    State currentState = State.ChoosingWorld;

    enum World { Jabir, Jordan, Morgan, Nadia, Ben, Matthew, Zack, Dante, Ian }; //crucially names of directories for internal usage only
    enum Level { Scene1, Scene2, Scene3, Scene4, Scene5 };



    List<Level> Scenes = new List<Level>();
    Dictionary<World, List<Level>> levels = new Dictionary<World, List<Level>>();


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
            List<Level> sameLevelList = new List<Level>() { Level.Scene1, Level.Scene2, Level.Scene3, Level.Scene4, Level.Scene5 };

   

           // levels.Add(World.Jabir, sameLevelList);
           // levels.Add(World.Jabir, sameLevelList);
           // levels.Add(World.Jabir, sameLevelList);
           // levels.Add(World.Jabir, sameLevelList);
           // levels.Add(World.Jabir, sameLevelList);

         }
        else 
        {
            //definitely a unity specific issue
            if (this != _instance)
                Destroy(this.gameObject);
        }


    }

    void OnGUI(){

        //initialize various sizes
        Vector2 padding = new Vector2(PercentHeightToPixel(.05f), PercentHeightToPixel(.05f));
        float fontGameTitleHeight = PercentHeightToPixel(.10f);
        float fontWorldTitleHeight = PercentHeightToPixel(.06f);

        GUIStyle myLabelGameTitleStyle = new GUIStyle(GUI.skin.label);
        myLabelGameTitleStyle.fontSize = (int)fontGameTitleHeight;
        

        GUIStyle myLabelWorldTitleStyle = new GUIStyle(GUI.skin.label);
        myLabelWorldTitleStyle.fontSize = (int)fontWorldTitleHeight;


        const string gameTitle = "BLOCK-IT";
        GUI.skin.label = myLabelGameTitleStyle;
        Vector2 gameTitleSz = GUI.skin.label.CalcSize(new GUIContent(gameTitle));


        const string worldTitle = "WORLD SELECTOR";
        GUI.skin.label = myLabelWorldTitleStyle;
        Vector2 worldTitleSz = GUI.skin.label.CalcSize(new GUIContent(worldTitle));

        string specificWorld = "";
        Vector2 specificWorldSz = GUI.skin.label.CalcSize(new GUIContent(specificWorld));




        //draw level selector components
        Rect rectLabelGameTitle = new Rect(padding.x, padding.y, padding.x + gameTitleSz.x, padding.y + gameTitleSz.y);
        GUI.skin.label = myLabelGameTitleStyle;
        GUI.Label(rectLabelGameTitle, gameTitle);



        Rect rectWorldTitle = new Rect(Screen.width - worldTitleSz.x - padding.x, rectLabelGameTitle.height - worldTitleSz.y, padding.x + worldTitleSz.x, padding.y + worldTitleSz.y);
        GUI.skin.label = myLabelWorldTitleStyle;
        GUI.Label(rectWorldTitle, worldTitle);

        GUI.Box(new Rect(padding.x, padding.y + rectLabelGameTitle.height, Screen.width - padding.x * 2, Screen.height - padding.y * 2 - rectLabelGameTitle.height), "");

        float used = Screen.width - (gameTitleSz.x + worldTitleSz.x);
        Debug.Log(Screen.width + " " + gameTitleSz + worldTitleSz + " " + used + " " +used/Screen.width);

        float heightCounter = padding.x;
        float widthCounter = padding.y;



    }

    //@percentage = 0.001 to 1.000
    private float PercentHeightToPixel(float percentage)
    {
        //const float minScreenHeight = 180f;
        const float minPercentage = 0.05f;
        const float maxPercentage = 1f;

        if (percentage < minPercentage) 
        {
            Debug.LogWarning("Percentage input <color=maroon>too small</color>, increased from " + percentage + " to " + minPercentage); 
            percentage = minPercentage;
        }
        else if (percentage > maxPercentage)
        {
            Debug.LogWarning("Percentage input <color=maroon>too small</color>, <color=orange>increased from " + percentage + " to " + maxPercentage);
            percentage = maxPercentage;
        }
        float pixels = percentage * Screen.height;
        return pixels;
    }
	
}


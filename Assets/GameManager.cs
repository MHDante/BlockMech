<<<<<<< HEAD
﻿using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

public class GameManager : MonoBehaviour
{

    private static GameManager _instance;

    enum SelectionState { NoSelector, WorldSelector, LevelSelector };

    SelectionState selState = SelectionState.WorldSelector;

    List<string> worlds { get {
        return worldScenes.Keys.ToList(); 
    } } //ian question: why is this return statement valid in the class-space? a method is executing before inititialize if a get is executed? ... however, it seems this get would never actually execute before it's allowed... i need help conceptualizing why this is valid to access ... 

    List<string> levels { get 
    {
        List<string> s = worldScenes[selectedWorld];
        return s; 
    } }

    Dictionary<string, List<string>> worldScenes;

    string selectedWorld;

    

    public static GameManager instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.Find("GameManager").GetComponent<GameManager>();
                DontDestroyOnLoad(_instance.gameObject);
                _instance.init();
            }
            return _instance;
        }
    }


    void Awake()
    {

        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(_instance);
            init();

        }
        else
        {
            //definitely a unity specific issue: ZHARRIS;
            if (this != _instance)
                Destroy(this.gameObject);
        }


    }


    void init()
    {
        //worlds = ReadSceneNames.instance.worlds.ToList();            
        //    // = ReadSceneNames.instance.scenesD.Keys.ToList(); //DOESN'T WORK: DANTE SAYS UNITY WON'T SERIALIZE

        //ArraysToDictionaryMagic atdm = new ArraysToDictionaryMagic(ReadSceneNames.instance.worlds, ReadSceneNames.instance.scenes);
        //worldScenes = atdm.get();

        ArraysToDictionaryMagic atdm = new ArraysToDictionaryMagic(ReadSceneNames.instance);
        worldScenes = atdm.getScenes;
        if (worlds.Count == 0) { Debug.LogWarning("No scenes detected. Please click <color=magenta>Update Scene Names</color>."); }

        selectedWorld = "";
    }


    

    void OnGUI(){

        if (selState == SelectionState.WorldSelector)
        {
            Draw(worlds);
        }
        else if (selState == SelectionState.LevelSelector)
        {
            Draw(levels);
        }
		
    }


    //@percentage = 0.001 to 1.000
    private float PercentToPixel(float percentage, float relativeTo)
    {
        //const float minScreenHeight = 180f;
        const float minPercentage = 0.04f;
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
        float pixels = percentage * relativeTo;
        return pixels;
    }


    Vector2 scrollPosition = Vector2.zero;

    void Draw(List<string> boxesToSelect)
    {
        //initialize various sizes
        Vector2 padding = new Vector2(PercentToPixel(.05f, Screen.height), PercentToPixel(.05f, Screen.height));
        float fontGameTitleHeight = PercentToPixel(.10f, Screen.height);
        float fontWorldTitleHeight = PercentToPixel(.06f, Screen.height);
        float fontButtonHeightMax = PercentToPixel(.04f, Screen.height);

        GUIStyle myLabelGameTitleStyle = new GUIStyle(GUI.skin.label);
        myLabelGameTitleStyle.fontSize = (int)fontGameTitleHeight;


        GUIStyle myLabelWorldTitleStyle = new GUIStyle(GUI.skin.label);
        myLabelWorldTitleStyle.fontSize = (int)fontWorldTitleHeight;


        GUIStyle myLabelButtonTitleStyle = new GUIStyle(GUI.skin.button);
        myLabelButtonTitleStyle.fontSize = (int)fontButtonHeightMax;

        const string gameTitle = "BLOCK-IT";
        GUI.skin.label = myLabelGameTitleStyle;
        Vector2 gameTitleSz = GUI.skin.label.CalcSize(new GUIContent(gameTitle));


        const string worldTitle = "WORLD SELECTOR";
        GUI.skin.label = myLabelWorldTitleStyle;
        Vector2 worldTitleSz = GUI.skin.label.CalcSize(new GUIContent(worldTitle));

        string specificWorld = "";
        Vector2 specificWorldSz = GUI.skin.label.CalcSize(new GUIContent(specificWorld)); //rename you later WorldSz


        Rect rectLabelGameTitle = new Rect(padding.x, padding.y, padding.x + gameTitleSz.x, padding.y + gameTitleSz.y);

        Rect rectWorldTitle = new Rect(Screen.width - worldTitleSz.x - padding.x, rectLabelGameTitle.height - worldTitleSz.y, padding.x + worldTitleSz.x, padding.y + worldTitleSz.y);

        Rect rectPosition = new Rect(padding.x, padding.y + rectLabelGameTitle.height, Screen.width - padding.x * 2, Screen.height - padding.y * 2 - rectLabelGameTitle.height);

        Vector2 levelSelectorSz = new Vector2(PercentToPixel(.18f, Screen.width), PercentToPixel(.18f, Screen.width)); //this can be more versatile, it's hacky in that it only depends on width to calculate it's dimensions.

        int viewportReduction = 20;
        int maxCol = (int)Mathf.Floor((float)(rectPosition.width - viewportReduction - padding.x * 3) / (float)(levelSelectorSz.x + padding.x)); //4
        int maxRow;
       // if (boxesToSelect.Count < maxCol)
       // {
       //     maxRow = (int)Mathf.Ceil((float)maxCol / (float)boxesToSelect.Count);
       // }
       // else
       // {
            maxRow = (int)Mathf.Ceil((float)boxesToSelect.Count / (float)maxCol);
       // }

        Rect rectViewport = new Rect(0, 0, rectPosition.width - viewportReduction, ((levelSelectorSz.y + padding.y) * maxRow + padding.y * 2) + padding.y);



        //draw game title 
        GUI.skin.label = myLabelGameTitleStyle;
        GUI.Label(rectLabelGameTitle, gameTitle);


        //draw world title placeholder
        GUI.skin.label = myLabelWorldTitleStyle;    //not my favourite variable name... meh...
        GUI.Label(rectWorldTitle, worldTitle);






        //**DRAW START**
        //draw level selector components
        scrollPosition =
        GUI.BeginScrollView(rectPosition, scrollPosition, rectViewport);
        GUI.Box(rectViewport, "");


        int c = -1;
        for (int box = 0; box < boxesToSelect.Count; box++) 
        {
            int r = box % maxCol;
            if (r == 0) c++;

            Vector2 spawn = new Vector2(padding.x * 2 + r * (levelSelectorSz.x + padding.x), padding.y * 2 + c * (levelSelectorSz.y + padding.y));
            float colSpaceWasted = rectViewport.width - (padding.x * 4 + maxCol * (levelSelectorSz.x + padding.x) - padding.x); 

            string labelText = boxesToSelect[box].ToString();
            if (GUI.Button(new Rect(spawn.x + colSpaceWasted/2, spawn.y, levelSelectorSz.x, levelSelectorSz.y), labelText.ToUpper(), myLabelButtonTitleStyle))
            {
                if (selState == SelectionState.WorldSelector)
                {
                    //set global variable, next iteration of main game loop would cause different list menu
                    selectedWorld = labelText;
                    selState = SelectionState.LevelSelector;
                }
                else if (selState == SelectionState.LevelSelector)
                {

                }
                

                //Application.LoadLevel( ReadSceneNames.instance.scenes[ selectedWorld ][] );
                //Debug.Log("<color=green>Loading level " + foundScene + "</color>");


            }

        }


        GUI.EndScrollView();
        //**DRAW END**


        //**MOVE START**
        //move according to scroll wheel
        float scrollMovement = 3.33f * levelSelectorSz.y / 4;
        float result = Input.GetAxis("Mouse ScrollWheel");
        if (result != 0)
        {
            scrollPosition = new Vector2(scrollPosition.x, scrollPosition.y + result * -scrollMovement);
        }

        foreach (var T in Input.touches)
        {

        }
        //**MOVE END**

    }

}
=======
﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

    private static GameManager _instance;

    enum State { Splash, ChoosingWorld, ChoosingLevel, Playing }
    State currentState = State.ChoosingWorld;

    enum World { Jabir, Jordan, Morgan, Nadia, Ben, Matthew, Zack, Dante, Ian }; //crucially names of directories for internal usage only
    enum Level { Scene1, Scene2, Scene3, Scene4, Scene5 };


    Dictionary<World, List<Level>> levels = new Dictionary<World, List<Level>>();

    bool showWorld;
    World selectedWorld = (World)(-1);

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
            List<Level> sameScenes = new List<Level>() { Level.Scene1, Level.Scene2, Level.Scene3, Level.Scene4, Level.Scene5 };

            int eSize = System.Enum.GetNames(typeof(World)).Length;

            for (int world = 0; world < eSize; world++) {
                levels.Add((World)world, sameScenes);
            }
            showWorld = true;

            Object[] scenes = Resources.LoadAll("Scenes");
            string s = scenes[1].name;
            //Application.LoadLevel(s);


        }
        else 
        {
            //definitely a unity specific issue: ZHARRIS;
            if (this != _instance)
                Destroy(this.gameObject);
        }


    }

    Vector2 scrollPosition = Vector2.zero;

    void OnGUI(){

        //initialize various sizes
        Vector2 padding = new Vector2(PercentToPixel(.05f, Screen.height), PercentToPixel(.05f, Screen.height));
        float fontGameTitleHeight = PercentToPixel(.10f, Screen.height);
        float fontWorldTitleHeight = PercentToPixel(.06f, Screen.height);
        float fontButtonHeightMax = PercentToPixel(.04f, Screen.height);

        GUIStyle myLabelGameTitleStyle = new GUIStyle(GUI.skin.label);
        myLabelGameTitleStyle.fontSize = (int)fontGameTitleHeight;
        

        GUIStyle myLabelWorldTitleStyle = new GUIStyle(GUI.skin.label);
        myLabelWorldTitleStyle.fontSize = (int)fontWorldTitleHeight;


        GUIStyle myLabelButtonTitleStyle = new GUIStyle(GUI.skin.button);
        myLabelButtonTitleStyle.fontSize = (int)fontButtonHeightMax;

        const string gameTitle = "BLOCK-IT";
        GUI.skin.label = myLabelGameTitleStyle;
        Vector2 gameTitleSz = GUI.skin.label.CalcSize(new GUIContent(gameTitle));


        const string worldTitle = "WORLD SELECTOR";
        GUI.skin.label = myLabelWorldTitleStyle;
        Vector2 worldTitleSz = GUI.skin.label.CalcSize(new GUIContent(worldTitle));

        string specificWorld = "";
        Vector2 specificWorldSz = GUI.skin.label.CalcSize(new GUIContent(specificWorld));


        //find largest button text size

        Vector2 longestTextSz = Vector2.zero;
        string longestText = "";
        foreach(KeyValuePair<World, List<Level>> world in levels)
        {
            string s = world.Key.ToString().ToUpper();
            Vector2 checkLargestButtonTextSz = GUI.skin.label.CalcSize(new GUIContent( s ));
            if (checkLargestButtonTextSz.x > longestTextSz.x)  //disregards height deliberately
            { 
                longestTextSz = checkLargestButtonTextSz;
                longestText = s;
            }
        }


        
        Vector2 checkLargestLabelWithTextSz = GUI.skin.label.CalcSize(new GUIContent( longestText )); //lol scope sigh, can't overwrite, can't declare again. wtf.
        
        //check if longestTextSz exceeds button width, if so reduce until fits.
        Vector2 levelSelectorSz = new Vector2(PercentToPixel(.18f, Screen.width), PercentToPixel(.18f, Screen.width));
        if (checkLargestLabelWithTextSz.x > (int)levelSelectorSz.x ) {
            //Debug.Log("MAKE ALGORITHM HERE. " + checkLargestLabelWithTextSz.x + " " + (int)levelSelectorSz.x);
        }


         
        


        //draw game title 
        Rect rectLabelGameTitle = new Rect(padding.x, padding.y, padding.x + gameTitleSz.x, padding.y + gameTitleSz.y);
        GUI.skin.label = myLabelGameTitleStyle;
        GUI.Label(rectLabelGameTitle, gameTitle);



        //figure out amount of rows, use to make inner scroll box height;
        //
        int eSize = System.Enum.GetNames(typeof(World)).Length;
        int maxCol = 4;
        int maxRow = (int)Mathf.Ceil((float)eSize / (float)maxCol);

        string title;
        if (showWorld)
        {
            title = worldTitle;
        }
        else 
        {
            title = selectedWorld.ToString().ToUpper() + "'S WORLD";
        }

        //draw world title placeholder
        Rect rectWorldTitle = new Rect(Screen.width - worldTitleSz.x - padding.x, rectLabelGameTitle.height - worldTitleSz.y, padding.x + worldTitleSz.x, padding.y + worldTitleSz.y);
        GUI.skin.label = myLabelWorldTitleStyle;
        GUI.Label(rectWorldTitle, worldTitle);

        Rect rectSelectorGroup = new Rect(padding.x, padding.y + rectLabelGameTitle.height, Screen.width - padding.x * 2, Screen.height - padding.y * 2 - rectLabelGameTitle.height);
        Rect rectSelectorBox = new Rect(0, 0, Screen.width - padding.x , ((levelSelectorSz.y + padding.y) * maxRow + padding.y * 2 ));



        //draw level selector components
        scrollPosition = 
        GUI.BeginScrollView(rectSelectorGroup, scrollPosition, rectSelectorBox);

        GUI.Box(rectSelectorBox, "");


        int c = -1;
        for (int world = 0; world < levels.Count; world++) //
        {
            int maxRowSz = 4;//... magic...
            int r = world % maxRowSz;
            if (r == 0) c++;
            //Debug.Log(r + " " + c);
            Vector2 spawn = new Vector2(padding.x * 2 + r * (levelSelectorSz.x + padding.x), padding.y * 2 + c * (levelSelectorSz.y + padding.y));
            Vector2 levelSelectorPos = new Vector2(spawn.x, spawn.y);
            //  45% for margins, 55% for buttons.
            // margins: 30 px * 9 = 270 px.   buttons 172 px * 4 = 688 px.   total screen calc = 958 
            // margin: 3.13 %                 button: 18%
            // 18 18 18 18 3.15 3.15 3.15 etc.

            if (GUI.Button(new Rect(levelSelectorPos.x, levelSelectorPos.y, levelSelectorSz.x, levelSelectorSz.y), ((World)world).ToString().ToUpper(), myLabelButtonTitleStyle))
            {
                DrawWorldMenu((World)world);
                Debug.Log(world + " pressed");
            }
        }


        GUI.EndScrollView();

        //move according to scroll wheel
        float scrollMovement = 3.33f * levelSelectorSz.y/4;
        float result = Input.GetAxis("Mouse ScrollWheel");
        if (result != 0)
        {
            scrollPosition = new Vector2(scrollPosition.x, scrollPosition.y + result * -scrollMovement);
        }



        //move according to touches on screen

        foreach (var T in Input.touches)
        { 
            
        }


        //GUI.Box(new Rect(padding.x, padding.y + rectLabelGameTitle.height, Screen.width - padding.x * 2, Screen.height - padding.y * 2 - rectLabelGameTitle.height), "");

        //float used = Screen.width - (gameTitleSz.x + worldTitleSz.x);
        //Debug.Log(Screen.width + " " + gameTitleSz + worldTitleSz + " " + used + " " +used/Screen.width);


    }

    //@percentage = 0.001 to 1.000
    private float PercentToPixel(float percentage, float relativeTo)
    {
        //const float minScreenHeight = 180f;
        const float minPercentage = 0.04f;
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
        float pixels = percentage * relativeTo;
        return pixels;
    }

    private void DrawWorldMenu(World world) {

        selectedWorld = world;

    }

    
	
}

>>>>>>> origin/master

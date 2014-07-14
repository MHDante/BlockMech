using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

public class GameManager : MonoBehaviour {

    private static GameManager _instance;

    enum State { Splash, ChoosingWorld, ChoosingLevel, Playing }
    State currentState = State.ChoosingWorld;

    enum World { Jabir, Jordan, Morgan, Nadia, Ben, Matthew, Zack, Dante, Ian }; //these world's represent directories and are just fluff, i.e., not important functionally, just present as reminders to make puzzles.
    

    Dictionary<string, List<string>> levels = new Dictionary<string, List<string>>();

    bool showWorld;
    string selectedWorld;
    private string targetScene;
    List<string> foundWorlds;

    public static GameManager instance {
        get {
            if (_instance == null)
            {
                _instance = GameObject.Find("GameManager").GetComponent<GameManager>();
                DontDestroyOnLoad(_instance.gameObject);
                _instance.init();
            }
            return _instance;
        }
    }


    void Awake(){

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
        //main game manager logic goes here
        List<string> sameScenes = new List<string>();

        //parse all individual names into a list<string> called worlds
        //parse all world's numbers afterwards.
        List<string> buildSettingScenes = (ReadSceneNames.instance.scenes).ToList<string>();
        string pattern = @"^[A-Z]*";
        foundWorlds = new List<string>();
        Regex rgx = new Regex(pattern, RegexOptions.IgnoreCase);
        foreach (string sceneString in buildSettingScenes)
        {
            MatchCollection matches = rgx.Matches(sceneString);
            foreach (Match match in matches)
            {
                string key = match.Value;
                int startPos = match.Value.Length;
                string leftOver = sceneString.Substring(startPos);
                string value = leftOver;

                if (!foundWorlds.Contains(match.Value))
                {
                    
                    //adds empty list to dictionary
                    levels.Add(key, new List<string>());
                    if (!levels[key].Contains(value)) 
                    {
                        //adds first entry to dictionary
                        levels[key].Add(value);
                    }
                    else 
                    {
                        Debug.Log("Duplicate scene found: " + key+value );
                    }

                }
                else 
                {
                    if (!levels[key].Contains(value))
                    {
                        levels[key].Add(value);
                    }
                    else
                    {
                        Debug.Log("Duplicate scene found: " + key + value);
                    }
                }
                
            }
        }


        //List<string> b = buildSettingScenes.Distinct();
        //
        //for (int world = 0; world < eSize; world++) {
        //    levels.Add((World)world, sameScenes);
        //}
        showWorld = true;
    }

    Vector2 scrollPosition = Vector2.zero;

    void OnGUI(){

        

        if (showWorld)
        {


            List<string> levelsForThatSelection = foundWorlds;
            bool isTerminal = false;

            RecursiveSelect(levelsForThatSelection, isTerminal);




        } //end if (showWorld)
        else 
        {
            for (int level = 0; level < levels.Count; level++) 
            {

            }
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


    void RecursiveSelect (List<string> boxesToSelect, bool isTerminal)
    {
        Draw(boxesToSelect);
        if (!isTerminal)
        {
            boxesToSelect = new List<string>(){ "1", "2", "3" }; /*numbers found*/;
            isTerminal = true;
            RecursiveSelect( boxesToSelect, isTerminal );
        }
        else 
        { 
            return ; 
        }
    }

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
        Vector2 specificWorldSz = GUI.skin.label.CalcSize(new GUIContent(specificWorld));


        //find largest button text size



        Vector2 longestTextSz = Vector2.zero;
        string longestText = "";
        foreach (KeyValuePair<string, List<string>> world in levels)
        {
            string s = world.Key.ToString().ToUpper();
            Vector2 checkLargestButtonTextSz = GUI.skin.label.CalcSize(new GUIContent(s));
            if (checkLargestButtonTextSz.x > longestTextSz.x)  //disregards height deliberately
            {
                longestTextSz = checkLargestButtonTextSz;
                longestText = s;
            }
        }









        //draw game title 
        Rect rectLabelGameTitle = new Rect(padding.x, padding.y, padding.x + gameTitleSz.x, padding.y + gameTitleSz.y);
        GUI.skin.label = myLabelGameTitleStyle;
        GUI.Label(rectLabelGameTitle, gameTitle);



        //draw world title placeholder
        Rect rectWorldTitle = new Rect(Screen.width - worldTitleSz.x - padding.x, rectLabelGameTitle.height - worldTitleSz.y, padding.x + worldTitleSz.x, padding.y + worldTitleSz.y);
        GUI.skin.label = myLabelWorldTitleStyle;
        GUI.Label(rectWorldTitle, worldTitle);


        string title;
        if (showWorld)
        {
            title = worldTitle;
        }
        else
        {
            title = selectedWorld.ToString().ToUpper() + "'S WORLD";
        }

        Vector2 checkLargestLabelWithTextSz = GUI.skin.label.CalcSize(new GUIContent(longestText)); //lol scope sigh, can't overwrite, can't declare again. wtf.

        //check if longestTextSz exceeds button width, if so reduce until fits.
        Vector2 levelSelectorSz = new Vector2(PercentToPixel(.18f, Screen.width), PercentToPixel(.18f, Screen.width));
        if (checkLargestLabelWithTextSz.x > (int)levelSelectorSz.x)
        {
            //Debug.Log("MAKE ALGORITHM HERE. " + checkLargestLabelWithTextSz.x + " " + (int)levelSelectorSz.x);
        }


        Rect rectPosition = new Rect(padding.x, padding.y + rectLabelGameTitle.height, Screen.width - padding.x * 2, Screen.height - padding.y * 2 - rectLabelGameTitle.height);

        //figure out amount of rows, use to make inner scroll box height;
        //
        int wSize = foundWorlds.Count;
        int maxCol = (int)Mathf.Ceil((float)rectPosition.width / (float)levelSelectorSz.x); //4
        int maxRow = (int)Mathf.Ceil((float)wSize / (float)maxCol);

        Rect rectViewport = new Rect(0, 0, rectPosition.width - 20, ((levelSelectorSz.y + padding.y) * maxRow + padding.y * 2));




        //**START**
        //draw level selector components
        scrollPosition =
        GUI.BeginScrollView(rectPosition, scrollPosition, rectViewport);

        GUI.Box(rectViewport, "");


        int c = -1;
        for (int box = 0; box < levels.Count; box++) //
        {
            int maxRowSz = 4; //... magic...
            int r = box % maxRowSz;
            if (r == 0) c++;
            //Debug.Log(r + " " + c);
            Vector2 spawn = new Vector2(padding.x * 2 + r * (levelSelectorSz.x + padding.x), padding.y * 2 + c * (levelSelectorSz.y + padding.y));
            Vector2 levelSelectorPos = new Vector2(spawn.x, spawn.y);

            if (GUI.Button(new Rect(levelSelectorPos.x, levelSelectorPos.y, levelSelectorSz.x, levelSelectorSz.y), box.ToString().ToUpper(), myLabelButtonTitleStyle))
            {

                
                RecursiveSelect()

                //clicked 
                showWorld = false;
                Debug.Log(box + " pressed");
                selectedWorld = foundWorlds[box];
                targetScene = box.ToString() + box;
                string foundScene = Array.Find(ReadSceneNames.instance.scenes, seeking => seeking == targetScene);
                Debug.Log(targetScene + ", " + foundScene);
                if (foundScene != null)
                {
                    Application.LoadLevel(foundScene);
                    Debug.Log("<color=green>Loading level " + foundScene + "</color>");
                }
            }
        }//end for (worlds)

        GUI.EndScrollView();
        //**END**


        //move according to scroll wheel
        float scrollMovement = 3.33f * levelSelectorSz.y / 4;
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

    
	
}


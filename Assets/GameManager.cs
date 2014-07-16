using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

public class GameManager : MonoBehaviour
{

    private static GameManager _instance;

    enum SelectionState { NoSelector, WorldSelector, LevelSelector, PlayingGame, ResultsScreen };

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
                    selState = SelectionState.PlayingGame;
                    Application.LoadLevel( worldScenes[ selectedWorld ][box] );
                    Debug.Log("<color=green>Loading level " + worldScenes[selectedWorld][box] + "</color>");
                }
               
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
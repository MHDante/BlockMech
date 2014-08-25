using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using OrbItUtils;


public enum GameState { NoSelector, Initializing, SplashScreen, WorldSelector, LevelSelector, PlayingGame, ResultsScreen };

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public GameState selState = GameState.Initializing;

    List<string> worlds { get {
        return sortedWorldScenes.ToDictionary(x => x.Key, x => x.Value).Keys.ToList();
        //return worldScenes.Keys.ToList();
        //return sortedWorldScenes.ToList();
    } } //ian question: why is this return statement valid in the class-space? a method is executing before inititialize if a get is executed? ... however, it seems this get would never actually execute before it's allowed... i need help conceptualizing why this is valid to access ... 

    List<string> levels { get 
    {
        List<string> s = worldScenes[selectedWorld];
        return s; 
    } }

    Dictionary<string, List<string>> worldScenes;

    List<KeyValuePair<string, List<string>>> sortedWorldScenes;




    string selectedWorld;
    Vector2 scrollPosition = Vector2.zero;
    const string worldTitle = "WORLD SELECTOR";
    Texture textureArrow;

    //initialize various sizes
    Vector2 padding;
    float fontGameTitleHeight;
    float fontWorldTitleHeight;
    float fontButtonHeightMax;
    float backButtonHeight;
    float backButtonPadding;

    GUIStyle myLabelGameTitleStyle;
    GUIStyle myLabelWorldTitleStyle;
    GUIStyle myLabelButtonTitleStyle;

    const string gameTitle = "BLOCK-IT";
    Vector2 gameTitleSz;
    Vector2 selectionTitleSz;

    // string specificWorld = "";
    // Vector2 specificWorldSz = GUI.skin.label.CalcSize(new GUIContent(specificWorld)); //rename you later WorldSz


    Rect rectLabelGameTitle;
    Rect rectSelectionTitle;
    Rect rectPosition;
    Vector2 levelSelectorSz;

    int viewportReduction = 20;
    int maxCol;
    int maxRow;

    Rect rectViewport;
    Vector2 backButtonSz;
    bool onceDone = false;


    //splash screen
    Texture texPuz;
    Texture texTitle;
    
    //results screen
    Texture texResults;
    Rect rectSteps;
    Rect rectTime;
    bool mustSwitchLevel;


    //player statistics
    public int totalSteps;
    public int levelSteps;
    public int totalRestarts;

    public Player.Stats latestStats;
    List<Player.Stats> listStats;


    
    public static GameManager instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject find = GameObject.Find("GameManager");
                if (find == null) return null;
                _instance = find.GetComponent<GameManager>();
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
        worldScenes = atdm.GetScenes;


        List<KeyValuePair<string, List<string>>> tempList = worldScenes.ToList();

        sortedWorldScenes = tempList.OrderByDescending(v => v.Value.Count).ThenBy(k => k.Key).ToList();



        if (worlds.Count == 0) { Debug.LogWarning("No scenes detected. Please click <color=magenta>Update Scene Names</color>."); }

        selectedWorld = "";
        textureArrow = Resources.Load<Texture>("Textures/arrow");

        //splash
        texPuz = Resources.Load<Texture>("Textures/coverPuzzleRotatedPlasticWrapNon");
        texTitle = Resources.Load<Texture>("Textures/BLOCKIT_TITLE_2_WC_DS");
        texResults = Resources.Load<Texture>("Texture/RESULTS_1234");

        //results
        mustSwitchLevel = true;

        //statistics
        totalSteps = 0;
        levelSteps = 0;
        totalRestarts = 0;

        listStats = new List<Player.Stats>();

    }
    void OnGUI(){

        if (selState == GameState.Initializing)
        {
            InitGUI();
            selState = GameState.SplashScreen;//.ResultsScreen;
        }
        else if (selState == GameState.SplashScreen)
        {
            DrawSplash();
            if (Input.anyKeyDown || Input.GetMouseButtonDown(0))
            {
                selState = GameState.WorldSelector;
            }
        }
        else if (selState == GameState.WorldSelector)
        {
            DrawTitle(worldTitle);
            DrawPicker(worlds);
        }
        else if (selState == GameState.LevelSelector)
        {
            DrawTitle(selectedWorld.ToUpper() + " WORLD");
            DrawPicker(levels);
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                selState = GameState.WorldSelector;
            }
        }
        else if (selState == GameState.PlayingGame) 
        {
            mustSwitchLevel = true;
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                string s = "<color=magenta>Exiting level.</color>";
                if (Player.stats != null) 
                {
                    s += " " + "<color=teal>Steps: " + Player.stats.steps + ". Time: " + Player.stats.TimeFormatted() + ".</color>"; 
                }
                Debug.Log(s);
                Application.LoadLevel("PuzzleSelector");
                selState = GameState.WorldSelector;
            }
        }
        else if (selState == GameState.ResultsScreen)
        {
            var i = Input.anyKeyDown;
            var m = Input.GetMouseButtonDown(0);
            if (i || m)
            {
                selState = GameState.LevelSelector;
            }
            if (mustSwitchLevel)
            {
                mustSwitchLevel = false;
                Application.LoadLevel("PuzzleSelector");
            }
            DrawTitle("RESULTS");
            DrawResults();

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


   


    void InitGUI() 
    {
        padding = new Vector2(PercentToPixel(.05f, Screen.height), PercentToPixel(.05f, Screen.height));
        fontGameTitleHeight = PercentToPixel(.10f, Screen.height);
        fontWorldTitleHeight = PercentToPixel(.06f, Screen.height);
        fontButtonHeightMax = PercentToPixel(.04f, Screen.height);
        backButtonHeight = PercentToPixel(.12f, Screen.height);
        backButtonPadding = backButtonHeight + padding.x;

        myLabelGameTitleStyle = new GUIStyle(GUI.skin.label);
        myLabelGameTitleStyle.fontSize = (int)fontGameTitleHeight;

        myLabelWorldTitleStyle = new GUIStyle(GUI.skin.label);
        myLabelWorldTitleStyle.fontSize = (int)fontWorldTitleHeight;

        myLabelButtonTitleStyle = new GUIStyle(GUI.skin.button);
        myLabelButtonTitleStyle.fontSize = (int)fontButtonHeightMax;

        GUI.skin.label = myLabelGameTitleStyle;
        gameTitleSz = GUI.skin.label.CalcSize(new GUIContent(gameTitle));

        

        rectLabelGameTitle = new Rect(padding.x, padding.y, padding.x + gameTitleSz.x, padding.y + gameTitleSz.y);

        
    }

    void DrawTitle(string selectionTitle) 
    {
        GUI.skin.label = myLabelWorldTitleStyle;
        selectionTitleSz = GUI.skin.label.CalcSize(new GUIContent(selectionTitle));

        rectSelectionTitle = new Rect(Screen.width - selectionTitleSz.x - padding.x, rectLabelGameTitle.height - selectionTitleSz.y, padding.x + selectionTitleSz.x, padding.y + selectionTitleSz.y);

        //draw game title 
        GUI.skin.label = myLabelGameTitleStyle;
        GUI.Label(rectLabelGameTitle, gameTitle);


        //draw world title placeholder
        GUI.skin.label = myLabelWorldTitleStyle;    //not my favourite variable name... meh...
        GUI.Label(rectSelectionTitle, selectionTitle);
    }


    void DrawPicker(List<string> boxesToSelect)
    {

        rectPosition = new Rect(padding.x + backButtonPadding, padding.y + rectLabelGameTitle.height, Screen.width - padding.x * 2 - backButtonPadding * 2, Screen.height - padding.y * 2 - rectLabelGameTitle.height);

        levelSelectorSz = new Vector2(PercentToPixel(.15f, Screen.width), PercentToPixel(.10f, Screen.width)); //this can be more versatile, it's hacky in that it only depends on width to calculate it's dimensions.

        maxCol = (int)Mathf.Floor((float)(rectPosition.width - viewportReduction - padding.x * 3) / (float)(levelSelectorSz.x + padding.x)); //4



        rectViewport = new Rect(0, 0, rectPosition.width - viewportReduction, ((levelSelectorSz.y + padding.y) * maxRow + padding.y * 2) + padding.y);

        backButtonSz = new Vector2(backButtonHeight, backButtonHeight);

        maxRow = (int)Mathf.Ceil((float)boxesToSelect.Count / (float)maxCol);






        //draw back button
        if (selState == GameState.LevelSelector)
        {
            if (GUI.Button(new Rect(padding.x, Screen.height - backButtonSz.y - padding.y, backButtonSz.x, backButtonSz.y), textureArrow, myLabelButtonTitleStyle))
            {
                selState = GameState.WorldSelector;
                selectedWorld = worldTitle;
            }
        }


        if (selState != GameState.Initializing && selState != GameState.ResultsScreen && onceDone)
        {

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
                if (GUI.Button(new Rect(spawn.x + colSpaceWasted / 2, spawn.y, levelSelectorSz.x, levelSelectorSz.y), labelText.ToUpper(), myLabelButtonTitleStyle))
                {
                    if (selState == GameState.WorldSelector)
                    {
                        //set global variable, next iteration of main game loop would cause different list menu
                        selectedWorld = labelText;
                        selState = GameState.LevelSelector;
                    }
                    else if (selState == GameState.LevelSelector)
                    {
                        selState = GameState.PlayingGame;
                        Application.LoadLevel(worldScenes[selectedWorld][box]);
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

            //foreach (var T in Input.touches)
            //{
            //
            //}
            //**MOVE END**
        }

        onceDone = true;

    }

    void DrawResults() {
        float w = (Screen.width * .80f) - padding.x * 4;
        float h = (Screen.height * .20f);// -padding.y * 4;

        Rect rectTitlePos = new Rect(padding.x * 2, padding.y * 2, w, h);
        Rect rectPos = new Rect(padding.x * 7, rectTitlePos.yMax - padding.y, Screen.width, Screen.height - rectTitlePos.yMax);
        //GUI.DrawTexture(rectPos, texResults, ScaleMode.ScaleToFit);

        //SCORE
        Rect rectSteps = new Rect(rectLabelGameTitle.x, rectLabelGameTitle.yMax + padding.y, rectLabelGameTitle.width, rectLabelGameTitle.height);
        Rect rectTime = new Rect(rectLabelGameTitle.x, rectSteps.yMax + padding.y, rectLabelGameTitle.width, rectLabelGameTitle.height);

        GUI.skin.label = myLabelGameTitleStyle;

        GUI.Label(rectSteps, latestStats.steps.ToString() );

        GUI.Label(rectTime, latestStats.TimeFormatted() );


    }

    void DrawSplash() 
    {

        float w = (Screen.width * .80f) - padding.x * 4;
        float h = (Screen.height * .20f);// -padding.y * 4;

        Rect rectTitlePos = new Rect(padding.x * 2, padding.y * 2, w, h);

        Rect rectPos = new Rect(padding.x * 7, rectTitlePos.yMax - padding.y , Screen.width, Screen.height - rectTitlePos.yMax);
        GUI.DrawTexture(rectPos, texPuz, ScaleMode.ScaleToFit);

        
        GUI.DrawTexture(rectTitlePos, texTitle, ScaleMode.ScaleToFit);


    }


    void Stats()
    {

    }
}
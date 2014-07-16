using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


//Source: http://answers.unity3d.com/questions/33263/how-to-get-names-of-all-available-levels.html
//Modified: per Zacks' request to parse any scene names but stored in a requisite folder

public class ReadSceneNames : MonoBehaviour
{
    private static ReadSceneNames _instance;
    public List<string> zNames, zLevels;
    public List<int> zIndexes;
    public static ReadSceneNames instance
    {
        get {
            if (_instance == null)
            {
                _instance = GameObject.Find("GameManager").GetComponent<ReadSceneNames>();
                //tell unity to not destory this object when loading this scene!
                DontDestroyOnLoad(_instance.gameObject);
            }
            return _instance;
        }
    }

    void Awake()
    {
        //If I am the first instance, make me the Singleton
        if (_instance == null) {
            _instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            //really weird Unity-specific case according to Zack.
            if (this != _instance)
            {
                Destroy(this.gameObject);
            }
        }
    }
    void Start()
    {
        if (!Application.isPlaying)
        {
            ReadNames(this);
        }
    }
    

    //public Dictionary<string, List<string>> scenesD; //DOESN'T WORK. DANTE SAYS WON'T SERIALIZE
    //public string[][] scenes;
    //public string[] worlds;

#if UNITY_EDITOR
    private static void ReadNames(ReadSceneNames inst)
    {
        //List<string> temp = new List<string>();
        Dictionary<string, List<string>> tempMaster = new Dictionary<string, List<string>>();
        List<string> temp = new List<string>();

        foreach (UnityEditor.EditorBuildSettingsScene scene in UnityEditor.EditorBuildSettings.scenes)
        {
            if (scene.enabled)
            {
                //char slash = Path.DirectorySeparatorChar; //problem 1: Path not resolved. p2: worry about web deployment of this library.
                int lastIndex = scene.path.LastIndexOf('/') + 1;
                string name = scene.path.Substring(lastIndex);
                string requiredFolder = scene.path.Substring(name.LastIndexOf('/') + 1);
                requiredFolder = requiredFolder.Substring(0, requiredFolder.Length - name.Length - 1);
                name = name.Substring(0, name.Length - 6);
                requiredFolder = requiredFolder.Substring(requiredFolder.LastIndexOf('/') + 1);
                temp.Add(name);

                if (!tempMaster.ContainsKey(requiredFolder))
                {
                    tempMaster.Add(requiredFolder, new List<string>());
                }
                if (!tempMaster[requiredFolder].Contains(name)) 
                { 
                    tempMaster[requiredFolder].Add(name);
                }
             
            }
        }
        DictionaryToArraysMagic dtam = new DictionaryToArraysMagic(tempMaster);
        inst.zNames = dtam.getNames;
        inst.zLevels = dtam.getLevels;
        inst.zIndexes = dtam.getIndexes;

        //instance.scenesD = tempMaster;  //DOESN'T WORK. DANTE SAYS WON'T SERIALIZE
        //DictionaryToArraysMagic dtam = new DictionaryToArraysMagic(tempMaster);
        //instance.scenes = dtam.get2d();
        //instance.worlds = dtam.get1d();
        ////return dtam.get2d();
        //
        //return dtam.get1d();    //irrelevant. just displays on screen

        //return tempMaster; //temp.ToArray();
    }
    [UnityEditor.MenuItem("CONTEXT/ReadSceneNames/Update Scene Names")]
    private static void UpdateNames(UnityEditor.MenuCommand command)
    {
        ReadSceneNames context = (ReadSceneNames)command.context;
        //context.worlds = ReadNames();
        ReadNames(context);
    }

    private void Reset()
    {
        //scenes = ReadNames();
        //worlds = ReadNames();
        ReadNames(this);
    }
#endif
}

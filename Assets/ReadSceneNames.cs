using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//Source: http://answers.unity3d.com/questions/33263/how-to-get-names-of-all-available-levels.html
public class ReadSceneNames : MonoBehaviour
{
    private static ReadSceneNames _instance;

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

    public string[] scenes;
#if UNITY_EDITOR
    private static string[] ReadNames()
    {
        List<string> temp = new List<string>();
        foreach (UnityEditor.EditorBuildSettingsScene scene in UnityEditor.EditorBuildSettings.scenes)
        {
            if (scene.enabled)
            {
                string name = scene.path.Substring(scene.path.LastIndexOf('/') + 1);
                name = name.Substring(0, name.Length - 6);
                temp.Add(name);
            }
        }
        return temp.ToArray();
    }
    [UnityEditor.MenuItem("CONTEXT/ReadSceneNames/Update Scene Names")]
    private static void UpdateNames(UnityEditor.MenuCommand command)
    {
        ReadSceneNames context = (ReadSceneNames)command.context;
        context.scenes = ReadNames();
    }

    private void Reset()
    {
        scenes = ReadNames();
    }
#endif
}

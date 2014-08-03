

using System;
using System.Collections.Generic;
using UnityEngine;

namespace OrbItUtils
{


    public static partial class Utils
    {

        public static void NotImplementedException()
        {
            var go = new GameObject();
            go.AddComponent<PopUp>().Message = "Zack and Dante are lazy.";
        }

        public class PopUp : MonoBehaviour
        {
            public String Message = "Other Text should appear here.";

            public void OnGUI()
            {
                GUI.Button(new Rect(Screen.width/4f, Screen.height/3f, Screen.width/2f, Screen.height/3f), Message);
            }
        }

        public static Rect Contract(this Rect source, int amount)
        {
            return new Rect(source.x + amount/2f, source.y + amount/2f, source.width - amount, source.height - amount);
        }

        public static GameObject GetParent(this GameObject child, string name = null)
        {
            while (true)
            {
                if (child.transform.parent == null) return null;
                if (name == null || child.transform.parent.gameObject.name == name)
                    return child.transform.parent.gameObject;
                child = child.transform.parent.gameObject;
            }
        }

        public static bool HasParent(this GameObject child, string name)
        {
            return (child.GetParent(name) != null);
        }
    }

    public class WTFException : Exception
        {
            public const string WTF = "What the Fjord?!?!";

            public WTFException()
                : base(WTF)
            {
            }

            public WTFException(String s)
                : base(s)
            {
                Debug.Log(WTF);
            }

            public WTFException(Exception e)
                : base(WTF, e)
            {
            }

            public WTFException(String s, Exception e)
                : base(s, e)
            {
                Debug.Log(WTF);
            }
        }
    }

public class ArraysToDictionaryMagic
{
    public ArraysToDictionaryMagic(ReadSceneNames readSceneNames)
    {
        GetScenes = new Dictionary<string, List<string>>();
        int count = 0;
        for (int i = 0; i < readSceneNames.zNames.Count; i++)
        {
            string name = readSceneNames.zNames[i];
            List<string> levels = new List<string>();
            int levelCount = readSceneNames.zIndexes[i];
            for (int j = 0; j < levelCount; j++)
            {
                levels.Add(readSceneNames.zLevels[count++]);
            }
            GetScenes[name] = levels;
        }
    }

    public Dictionary<string, List<string>> GetScenes { get; private set; }
}

public class DictionaryToArraysMagic
{
    public List<int> ZIndexes;
    public List<string> ZLevels;
    public List<string> ZNames;

    public DictionaryToArraysMagic(Dictionary<string, List<string>> masterScenes)
    {
        ZNames = new List<string>();
        ZLevels = new List<string>();
        ZIndexes = new List<int>();

        foreach (string name in masterScenes.Keys)
        {
            List<string> levelsAtName = masterScenes[name];
            if (levelsAtName.Count == 0) continue;

            ZNames.Add(name);
            ZIndexes.Add(levelsAtName.Count);
            foreach (string level in levelsAtName)
            {
                ZLevels.Add(level);
            }
        }
    }

    public List<int> GetIndexes
    {
        get { return ZIndexes; }
    }

    public List<string> GetLevels
    {
        get { return ZLevels; }
    }

    public List<string> GetNames
    {
        get { return ZNames; }
    }
}


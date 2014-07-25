using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using System.IO;

namespace Assets.Scripts.Editor
{
    public class LoadWizard : EditorWindow
    {
        [MenuItem("BlockIt/Load Wizardry %#d")]
        static void CreateEditor()
        {
            EditorWindow.GetWindow(typeof(LoadWizard));
        }
        string result = "";
        void OnGUI()
        {
            string path = Application.dataPath + "/SavedLevels";
            var infos = new DirectoryInfo(path);
            //we're leaving this here
            var fileinfos = infos.GetFiles().Select(f => f.Name).Where(s => !s.Contains(".meta")).Union(new string[]{""}).ToArray();
            result = fileinfos[EditorGUILayout.Popup("Choose Filename", fileinfos.ToList().IndexOf(result), fileinfos)];

            if (GUILayout.Button("Load"))
            {
                if (string.IsNullOrEmpty(result)) return;
                //Debug.Log(result);
                if(Application.isPlaying)
                    FileWrite.InitDeserialization(result);
            }
            
        }

    }
}

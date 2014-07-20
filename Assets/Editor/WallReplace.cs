using UnityEngine;
using UnityEditor;
using System.Collections;
// CopyComponents - by Michael L. Croswell for Colorado Game Coders, LLC
// March 2010

public class WallReplace : ScriptableWizard
{
    public GameObject Source;
    public GameObject Replacement;

    [MenuItem("BlockIt/Replace GameObjects")]
    static void CreateWizard()
    {
        ScriptableWizard.DisplayWizard("Replace GameObjects", typeof(WallReplace), "Replace");
    }

    void OnWizardCreate()
    {
        GameObject Destination = new GameObject();
        Destination.name = Source.name + "(Processed)";

        for (int i = 0; i < Source.transform.childCount; i++ )
        {
            GameObject oldObject = Source.transform.GetChild(i).gameObject;
            GameObject newObject = (GameObject)PrefabUtility.InstantiatePrefab(Replacement);
            newObject.transform.position = oldObject.transform.position;
            newObject.transform.rotation = oldObject.transform.rotation;
            if (oldObject.GetComponent<Wall>() != null && newObject.GetComponent<Wall>() != null)
            {
                EditorUtility.CopySerialized(oldObject.GetComponent<Wall>(), newObject.GetComponent<Wall>());
            }
            newObject.transform.parent = Destination.transform;
        }

    }
}

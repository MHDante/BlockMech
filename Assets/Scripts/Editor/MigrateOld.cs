using UnityEngine;
using UnityEditor;
using System.Collections;
using System;

public class MigrateOld : ScriptableWizard
{
    [MenuItem("BlockIt/Migrate Old")]
    static void CreateWizard()
    {
        ScriptableWizard.DisplayWizard("Replace GameObjects", typeof(MigrateOld), "Replace");
    }
    public GameObject BackgroundPrefab;
    private GameObject Scene;
    private GameObject MainCamera;
    private MetaData metaData;
    void OnWizardCreate()
    {
        if (!tryMigrate()) { 
            Debug.Log("FAILED."); 
        }
        else
        {
            Debug.Log("Success!");
        }
    }
    bool tryMigrate(){
        if (FindObjectsOfType<RoomManager>().Length != 1) return false;
        Scene = FindObjectsOfType<RoomManager>()[0].gameObject;
        if (FindObjectsOfType<Camera>().Length != 1) return false;
        MainCamera = FindObjectsOfType<Camera>()[0].gameObject;
        if (FindObjectsOfType<MetaData>().Length != 1) return false;
        metaData = FindObjectOfType<MetaData>();
        DestroyImmediate(GameObject.Find("Indicator"));
        Scene.transform.position = new Vector3(32,24,0);
        Scene.name = "Scene";
        DestroyImmediate(Scene.GetComponent<SpriteRenderer>());
        MainCamera.transform.parent = Scene.transform;

        EditorUtility.CopySerialized(metaData, Scene.AddComponent<MetaData>());
        DestroyImmediate(metaData.gameObject);
        GameObject TrippyBackground = (PrefabUtility.InstantiatePrefab(BackgroundPrefab) as GameObject);
        TrippyBackground.transform.parent = Scene.transform;
        TrippyBackground.transform.localRotation = default(Quaternion);
        TrippyBackground.transform.localScale = Vector3.one;
        TrippyBackground.transform.localPosition = default(Vector3);
        
        
        foreach (PieceType p in Enum.GetValues(typeof(PieceType)))
        {
            if (p == PieceType.Player || p == PieceType.None) continue;
            foreach (var o in FindObjectsOfType(RoomManager.pieceTypes[p])){
                MonoBehaviour mb = (MonoBehaviour)o;
                mb.transform.parent = PuzzleMaker.GetPieceParent(p).transform;
            }
        }
        return true;
    }
}
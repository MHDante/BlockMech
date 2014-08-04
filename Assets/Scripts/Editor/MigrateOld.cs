using OrbItUtils;
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System;

public class MigrateOld : ScriptableWizard
{
    [MenuItem("BlockIt/Migrate Old %e")]
    static void CreateWizard()
    {
        ScriptableWizard.DisplayWizard("Replace GameObjects", typeof(MigrateOld), "Replace");
    }
    void OnWizardCreate()
    {
        if (!tryMigrate())
        {
            Debug.Log("FAILED."); 
        }
        else
        {
            Debug.Log("Success!");
        }
    }



    private GameObject TrippyBackground;
    private GameObject Scene;
    private GameObject PuzzleMaster;
    private GameObject player;

    static Dictionary<string, int> namecounters = new Dictionary<string, int>();
    bool tryMigrate(){
        if (!GameObject.Find("LightEffect")) return false;
        TrippyBackground = GameObject.Find("LightEffect").GetParent();
        Scene = TrippyBackground.GetParent();
        if (!GameObject.Find("Puzzle_Pieces")) return false;
        PuzzleMaster = GameObject.Find("Puzzle_Pieces");
        player = GameObject.Find("Player") ?? GameObject.Find("player");
        if (!player) return player;
        player.name = "Player";
        DestroyImmediate(GameObject.Find("Indicator"));

        TrippyBackground.transform.parent = null;
        MetaData target = TrippyBackground.GetComponent<MetaData>() ?? TrippyBackground.AddComponent<MetaData>();
        EditorUtility.CopySerialized(Scene.GetComponent<MetaData>(), target);

        DestroyImmediate(Scene);
        
        TrippyBackground.name = "Scene";
        player.transform.parent = PuzzleMaster.transform;
        
        foreach (Type p in PuzzleMaker.PieceTypeList)
        {
            if (p == typeof(Player) || p == null ) continue;
            foreach (var o in FindObjectsOfType(p)){
                MonoBehaviour mb = (MonoBehaviour)o;
                if (namecounters.ContainsKey(mb.name))
                {
                    mb.name += "+" + ++namecounters[mb.name];
                }
                else
                {
                    namecounters[mb.name] = 0;
                }
            }
        }
        FileWrite.InitSerialization();
        return true;
    }
}
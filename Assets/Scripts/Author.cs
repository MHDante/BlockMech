using UnityEngine;
using System.Collections;
using System.Collections.Generic;
[ExecuteInEditMode]
public class MetaData : MonoBehaviour
{
    private static MetaData _instance;
    private static MetaData instance { get { if (_instance == null) _instance = (MetaData)Object.FindObjectOfType(typeof(MetaData)); return _instance; } }


    public enum difficulty_setting { Easy, Normal, Hard, Nightmare, Battletoads }

    public string author;
    public string levelName;
    public difficulty_setting difficulty;
    public bool usesLock;
    public bool usesBlock;
    public bool usesSwitch;
    public bool usesButton;
    public bool usesTrap;
    public bool usesTeleporter;

    public Dictionary<ColorSlot, Color> colors = new Dictionary<ColorSlot, Color>(){
        { ColorSlot.None, Utils.HexToColor("E2E2E2") },
        { ColorSlot.Gray, Utils.HexToColor("787679") },
        { ColorSlot.Purple, Utils.HexToColor("8569CF") },
        { ColorSlot.Blue, Utils.HexToColor("0D9FD8") },
        { ColorSlot.Green, Utils.HexToColor("8AD749") },
        { ColorSlot.Yellow, Utils.HexToColor("EECE00") },
        { ColorSlot.Orange, Utils.HexToColor("F8981F") },
        { ColorSlot.Red, Utils.HexToColor("F80E27") },
        { ColorSlot.Pink, Utils.HexToColor("F640AE") }
    };
    public static Color GetColorSlot(ColorSlot colorslot)
    {
        if (instance != null)
        {
            return instance.colors[colorslot];
        }
        return Color.grey;
    }


    void Awake()
    {
        _instance = this; //lol privaton
    }
   
}

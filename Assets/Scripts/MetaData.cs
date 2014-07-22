using UnityEngine;
using System.Collections;
using System.Collections.Generic;
[ExecuteInEditMode]
public class MetaData : MonoBehaviour
{
    private static MetaData _instance;
    private static MetaData instance { get { if (_instance == null) _instance = (MetaData)Object.FindObjectOfType(typeof(MetaData)); return _instance; } }
    private static int HintLimit = 32;

    public enum difficulty_setting { Easy, Normal, Hard, Nightmare, Battletoads }

    public string author;
    public string levelName;
    public string welcomeHint;

    public difficulty_setting difficulty;
    

    public static Dictionary<ColorSlot, Color> colors = new Dictionary<ColorSlot, Color>(){
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
            return colors[colorslot];
        }
        return Color.grey;
    }

    void OnValidate()
    {
        if (string.IsNullOrEmpty(welcomeHint)) welcomeHint = "BlockIt";
        if (welcomeHint.Length > HintLimit) welcomeHint = welcomeHint.Substring(0, HintLimit);
        GetComponentInChildren<TextMesh>().text = welcomeHint;
    }
    void Awake()
    {
        GetComponentInChildren<TextMesh>().text = welcomeHint;
        _instance = this; //lol privaton
    }
   
}

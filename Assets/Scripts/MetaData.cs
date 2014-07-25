using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
[ExecuteInEditMode]
public class MetaData : MonoBehaviour
{
    private static MetaData _instance;
    public static MetaData instance { get { if (_instance == null) _instance = (MetaData)FindObjectOfType(typeof(MetaData)); return _instance; } }
    private static int HintLimit = 32;

    public enum Difficulty { Easy, Normal, Hard, Nightmare, Battletoads }

    public string author;
    public string levelName;
    public string welcomeHint;

    public Difficulty difficulty;


    public static Dictionary<ColorSlot, string> colorCodes = new Dictionary<ColorSlot, string>(){
        { ColorSlot.None,   "E2E2E2" },
        { ColorSlot.Gray,   "787679" },
        { ColorSlot.Purple, "8569CF" },
        { ColorSlot.Blue,   "0D9FD8" },
        { ColorSlot.Green,  "8AD749" },
        { ColorSlot.Yellow, "EECE00" },
        { ColorSlot.Orange, "F8981F" },
        { ColorSlot.Red,    "F80E27" },
        { ColorSlot.Pink,   "F640AE" }
    };

    public static Dictionary<ColorSlot, Color> colors = colorCodes.Keys.ToDictionary(col => col, col => Utils.HexToColor(colorCodes[col]));

    public static Color GetColorSlot(ColorSlot colorslot)
    {
        return colors[colorslot];
    }

    void OnValidate()
    {
        if (string.IsNullOrEmpty(welcomeHint)) welcomeHint = "BlockIt";
        if (welcomeHint.Length > HintLimit) welcomeHint = welcomeHint.Substring(0, HintLimit);
        UpdateText(welcomeHint);
    }

    public void UpdateText(string text, ColorSlot color = ColorSlot.None)
    {
        TextMesh tm = GetComponentInChildren<TextMesh>();
        tm.color = GetColorSlot(color);
        tm.text = text;
    }
    public void ResetText()
    {
        UpdateText(welcomeHint);
    }

    void Awake()
    {
        GetComponentInChildren<TextMesh>().text = welcomeHint;
        _instance = this; //lol privaton //lol nope 
    }
   
}

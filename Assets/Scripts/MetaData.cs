﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using OrbItUtils;

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
    private string hintAppend = "Editing";

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
    public static ColorSlot GetColorFromSlot(Color color)
    {
        var slot = colors.First(kv => kv.Value == color);
        return slot.Key;
    }

    void OnValidate()
    {
        if (string.IsNullOrEmpty(welcomeHint)) welcomeHint = "BlockIt";
        if (welcomeHint.Length > HintLimit) welcomeHint = welcomeHint.Substring(0, HintLimit);
        UpdateText(GetHintString());
    }
    public void SetStateString(string state)
    {
        hintAppend = state;
        OnValidate();
    }

    public void UpdateText(string text, ColorSlot color = ColorSlot.None)
    {
        TextMesh tm = FindObjectOfType<TextMesh>();
        if (tm != null)
        {
            tm.color = GetColorSlot(color);
            tm.text = text;
        }
    }
    public string GetHintString()
    {
        return welcomeHint + (string.IsNullOrEmpty(hintAppend) ? "" : ("(" + hintAppend + ")"));
    }
    public void ResetText()
    {
        UpdateText(GetHintString());
    }
    void Awake()
    {
        GetComponentInChildren<TextMesh>().text = GetHintString();
        _instance = this; //lol privaton //lol nope 

    }
   
}

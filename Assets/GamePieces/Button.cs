using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
public class Button : GamePiece
{
    public override bool isSolid { get { return false; } set { } }
    public override bool isPushable { get { return false; } set { } }

    public override void Start()
    {
        base.Start();
    }
    public override void onDeOccupy(GamePiece piece)
    {
        base.onDeOccupy(piece);
        var list = RoomManager.roomManager.GetPiecesOfColor(colorslot);
        foreach (var colorPiece in list)
        {
            if (colorPiece is Keyhole) ((Keyhole)colorPiece).active = true;
        }
        Debug.Log(list.Count);
    }
    public override bool onOccupy(GamePiece piece)
    {
        var list = RoomManager.roomManager.GetPiecesOfColor(colorslot);
        bool allOccupied = true;
        foreach (var coloredPiece in list)
        {
            if (coloredPiece is Button && coloredPiece != this && !coloredPiece.IsOccupied) allOccupied = false;
        }
        if (allOccupied)
        {
            foreach (var colorPiece in list)
            {
                if (colorPiece is Keyhole) ((Keyhole)colorPiece).active = false;
            }
        }
        Debug.Log(list.Count);

        return true;
    }
    public override void Update()
    {
        base.Awake();
    }

    public void OnGUI()
    {
        if (Application.isPlaying && IsOccupied)
        {
            string s = GetTextFromFile("readthisfile");
            int a = 5;
            Rect r = new Rect(transform.position.x * a, transform.position.y * a, (transform.position.x + 100) * a, (transform.position.y + 100) * a);
            GUI.Label(r, s);
        }
    }
    public static Dictionary<string, string> fileTexts = new Dictionary<string, string>();
    public static string GetTextFromFile(string filename)
    {
        if (fileTexts.ContainsKey(filename))
        {
            return fileTexts[filename];
        }
        TextAsset file = Resources.Load<TextAsset>(filename);
        if (file != null)
        {
            fileTexts[filename] = file.text;
            return file.text;
        }
        return "filenotfound";
    }

}

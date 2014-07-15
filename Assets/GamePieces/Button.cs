using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
public class Button : GamePiece, Triggerable
{
    public override bool isSolid { get { return false; } set { } }
    public override bool isPushable { get { return false; } set { } }
    public bool IsTriggered { get { return IsOccupied; } }
    public override void Start()
    {
        base.Start();
        Transform activatedChild = transform.FindChild("Activated");
        if (activatedChild != null)
        {
            SpriteRenderer sr = activatedChild.GetComponent<SpriteRenderer>();
        }
    }
    public override void onDeOccupy(GamePiece piece)
    {
        base.onDeOccupy(piece);
        RoomManager.roomManager.RefreshColorFamily(colorslot);
        SetColorSlot(colorslot);
    }
    public override bool onOccupy(GamePiece piece)
    {
        RoomManager.roomManager.RefreshColorFamily(colorslot);
        SetColorSlot(colorslot);
        return true;
    }
    public override void Update()
    {
        base.Awake();
    }
    public void OnGUI()
    {

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

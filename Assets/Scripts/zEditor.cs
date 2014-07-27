using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class zEditor
{
    public zSidebar sidebar;
    public float sidebarScreenRation { get { return sidebarScreenRation; } }
    public zEditor(float sidebarScreenRation, float buttonWidth)
    {
        sidebar = new zSidebar(sidebarScreenRation, buttonWidth);
    }
    public void Draw()
    {
        sidebar.Draw();
    }
}

public class zSidebar
{
    List<zButton> contents;
    Rect properRect;
    Vector2 origin;
    float screenWidthPercent, width;

    public zButton piecePicker;
    public zButton colorPicker;
    public zButton eraserTool;
    public zButton selectTool;
    public zButton undoAction;
    public zButton menuButton;

    public zSidebar(float screenWidthPercent, float buttonWidth)
    {
        this.screenWidthPercent = screenWidthPercent;
        this.origin = new Vector2(Screen.width * (1f - screenWidthPercent), 0);
        this.width = Screen.width * screenWidthPercent;
        this.properRect = new Rect(origin.x, origin.y, width, Screen.height);
        this.contents = new List<zButton>();

        piecePicker = new zButton(typeof(Wall), buttonWidth, buttonWidth); contents.Add(piecePicker);
        colorPicker = new zButton(typeof(Tile), buttonWidth, buttonWidth); contents.Add(colorPicker);
        eraserTool = new zButton(typeof(Player), buttonWidth, buttonWidth); contents.Add(eraserTool);
        selectTool = new zButton(typeof(Key), buttonWidth, buttonWidth); contents.Add(selectTool);
        undoAction = new zButton(typeof(Button), buttonWidth, buttonWidth); contents.Add(undoAction);
        menuButton = new zButton("Menu", buttonWidth, buttonWidth); contents.Add(menuButton);

        float vertPadding = (Screen.height - (piecePicker.Height * contents.Count)) / (contents.Count + 1);
        float heightCounter = vertPadding;
        foreach(zButton btn in contents)
        {
            btn.x = Screen.width - (width / 2) - (btn.Width / 2);
            btn.y = heightCounter;
            heightCounter += btn.Height + vertPadding;
        }



    }

    public void Draw()
    {
        GUI.Box(properRect, "");
        foreach(var btn in contents)
        {
            btn.Draw();
        }
    }
}
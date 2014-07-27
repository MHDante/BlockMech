using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class Tester : MonoBehaviour {

    // Use this for initialization
    public GUISkin skin;
    zGridLayout pieceLayout, colorLayout;
    zEditor editor;
	void Start () {
        float sidebarScreenRatio = 0.1f;
        float buttonWidth = Screen.width * sidebarScreenRatio * 0.7f;
        editor = new zEditor(sidebarScreenRatio, buttonWidth);

        SetupPieceLayout(sidebarScreenRatio, buttonWidth);
        SetupColorLayout(sidebarScreenRatio, buttonWidth);
	}

    void SetupPieceLayout(float ratio, float buttonWidth)
    {
        List<zButton> zeButtons = new List<zButton>();
        foreach (Type t in RoomManager.PieceTypeList)
        {
            zeButtons.Add(new zButton(t, buttonWidth, buttonWidth));
        }
        float layoutwidth = Screen.width * .5f;
        float layoutheight = Screen.height * .6f;
        float layoutx = Screen.width * (1f - ratio) - layoutwidth;
        pieceLayout = new zGridLayout(new Rect(layoutx, 0, layoutwidth, layoutheight), zeButtons, true, new Vector2(Screen.width, 0), 1f);
        editor.sidebar.piecePicker.OnClick += (zb) =>
        {
            if (pieceLayout.IsVisible)
            {
                pieceLayout.Hide();
            }
        };
        editor.sidebar.piecePicker.LongPress += (zb) =>
        {
            if (pieceLayout.IsVisible)
            {
                pieceLayout.Hide();
            }
            else
            {
                if (colorLayout.IsVisible)
                {
                    colorLayout.Hide();
                }
                pieceLayout.Show();
            }
        };
        Action<zButton> hideLayout = (zb) => { pieceLayout.Hide(); editor.sidebar.piecePicker.type = zb.type; };
        foreach (var zb in pieceLayout.contents) { zb.OnClick += hideLayout; }
    }
    void SetupColorLayout(float ratio, float buttonWidth)
    {
        List<zButton> colorButtons = new List<zButton>();
        foreach (ColorSlot color in Enum.GetValues(typeof(ColorSlot)))
        {
            zButton zb = new zButton(typeof(Tile), buttonWidth, buttonWidth);
            zb.color = MetaData.colors[color];
            colorButtons.Add(zb);
            
        }
        float layoutwidth = Screen.width * .5f;
        float layoutheight = Screen.height * .4f;
        float layoutx = Screen.width * (1f - ratio) - layoutwidth;
        float layouty = colorButtons[0].Height + 12;
        colorLayout = new zGridLayout(new Rect(layoutx, layouty, layoutwidth, layoutheight), colorButtons, true, new Vector2(Screen.width, layouty), 1f);
        editor.sidebar.colorPicker.OnClick += (zb) =>
        {
            if (colorLayout.IsVisible)
            {
                colorLayout.Hide();
            }
        };
        editor.sidebar.colorPicker.LongPress += (zb) =>
        {
            if (colorLayout.IsVisible)
            {
                colorLayout.Hide();
            }
            else
            {
                if (pieceLayout.IsVisible)
                {
                    pieceLayout.Hide();
                }
                colorLayout.Show();
            }
        };
        Action<zButton> hideLayout = (zb) => { colorLayout.Hide(); editor.sidebar.colorPicker.color = zb.color; };//editor.sidebar.piecePicker.type = zb.type; };
        foreach (var zb in colorLayout.contents) { zb.OnClick += hideLayout; }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P)) pieceLayout.Show();
        if (Input.GetKeyDown(KeyCode.O)) pieceLayout.Hide();
    }
    void OnGUI()
    {
        GUI.skin = skin;
        pieceLayout.Draw();
        colorLayout.Draw();
        editor.Draw();
    }
}

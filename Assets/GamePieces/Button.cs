using UnityEngine;
using System.Collections;
using System;
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

}

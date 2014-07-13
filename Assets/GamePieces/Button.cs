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
    public override GamePiece onDeOccupy()
    {
        GamePiece piece = base.onDeOccupy();
        var list = RoomManager.roomManager.GetPiecesOfColor(colorslot);
        foreach (var colorPiece in list)
        {
            if (colorPiece is Keyhole) ((Keyhole)colorPiece).active = true;
        }
        Debug.Log(list.Count);
        return piece;
    }
    public override bool onOccupy(GamePiece piece)
    {
        bool success = base.onOccupy(piece);
        if (success)
        {
            var list = RoomManager.roomManager.GetPiecesOfColor(colorslot);
            bool allOccupied = true;
            foreach(var coloredPiece in list)
            {
                if (coloredPiece is Button && coloredPiece != this && !coloredPiece.IsOccupied) allOccupied = false;
            }
            if (allOccupied)
            {
                foreach(var colorPiece in list)
                {
                    if (colorPiece is Keyhole) ((Keyhole)colorPiece).active = false;
                }
            }
            Debug.Log(list.Count);
        }

        return success;
    }

}

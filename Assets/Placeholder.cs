using UnityEngine;
using System.Collections;
using System;
public class Placeholder : GamePiece
{
    public override bool isSolid { get { return true; } set { } }
    public override bool isPushable { get { return false; } set { } }

    public override void Start()
    {
        base.Start();
    }
    public override GamePiece onDeOccupy()
    {
        Debug.Log(containedPiece.name);
        return base.onDeOccupy();
    }
    public override bool onOccupy(GamePiece piece)
    {
        Debug.Log(piece.name);
        return base.onOccupy(piece);
    }

}

using UnityEngine;
using System.Collections;
using System;
public class Placeholder : GamePiece
{
    public override bool isSolid { get { return true; } set { } }
    public override bool isPushable { get { return true; } set { } }

    public override void Start()
    {
        base.Start();
    }
    public override void onDeOccupy(GamePiece piece)
    {
        base.onDeOccupy(piece);
    }
    public override bool onOccupy(GamePiece piece)
    {
        
        return base.onOccupy(piece);
    }

}

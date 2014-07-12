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
        return base.onDeOccupy();
    }
    public override bool onOccupy(GamePiece piece)
    {
        
        return base.onOccupy(piece);
    }

}

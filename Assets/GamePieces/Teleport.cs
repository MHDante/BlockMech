using UnityEngine;
using System.Collections;
using System;
public class Teleport : GamePiece
{
    public override bool isSolid { get { return true; } set { } }
    public override bool isPushable { get { return true; } set { } }
    public GameObject target;

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
        //if (target = null) Debug.LogError("Teleporter placed @ " + cell.x +", " + cell.y+" does not have a target set!");
        //piece.TeleportTo(target.GetComponent<GamePiece>().cell);
        return base.onOccupy(piece);
    }

}

using UnityEngine;
using System.Collections;
using System;
public class Teleport : GamePiece
{
    public override bool isSolid { get { return false; } set { } }
    public override bool isPushable { get { return false; } set { } }
    private float rotation = 0f, rotationRate = 1f;
    private int axisCounter = 0;
    public override void Start()
    {
        base.Start();
    }
    public override void Update()
    {
        base.Update();
        rotation = rotation + rotationRate;
        if (rotation > 360f)
        {
            rotation = 360f - rotation;
            axisCounter = (axisCounter + 1) % 6;
        }

        transform.eulerAngles = getAngleVector(rotation, Axis.Xaxis | Axis.Yaxis | Axis.Zaxis);
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

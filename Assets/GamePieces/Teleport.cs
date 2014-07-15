using UnityEngine;
using System.Collections;
using System;
public class Teleport : GamePiece
{
    public override bool isSolid { get { return false; } set { } }
    public override bool isPushable { get { return false; } set { } }
    private float rotation = 0f, rotationRate = 1f;
    private int axisCounter = 0;
    public GameObject target;
    public override void Start()
    {
        base.Start();
    }
    public override void Update()
    {
        base.Update();
        rotation = rotation + rotationRate;
        if (rotation > 180f)
        {
            rotation = 180f - rotation;
            axisCounter = (axisCounter + 1) % 7;
        }
        if (rendererActivated != null)
        {
            //transform.eulerAngles = getAngleVector(rotation, Axis.Xaxis | Axis.Yaxis | Axis.Zaxis);
            rendererActivated.transform.eulerAngles = getAngleVector(rotation, (Axis)(axisCounter + 1));
        }
        if (rendererColorized != null)
        {
            //transform.eulerAngles = getAngleVector(rotation, Axis.Xaxis | Axis.Yaxis | Axis.Zaxis);
            rendererColorized.transform.eulerAngles = getAngleVector(rotation, (Axis)((axisCounter + 4) % 7));
        }
    }
    
    public override void onDeOccupy(GamePiece piece)
    {
        base.onDeOccupy(piece);
    }
    public override bool onOccupy(GamePiece piece)
    {
        if (target == null)
        {
            Debug.Log("Teleporter placed @ " + cell.x + ", " + cell.y + " does not have a target set!");
        }
        else
        {
            Cell targetCell = target.GetComponent<GamePiece>().cell;
            piece.TeleportTo(targetCell);
        }
        return base.onOccupy(piece);
    }
}

using UnityEngine;
using System.Collections;
using System;
using System.Linq;
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
        axisCounter = UnityEngine.Random.Range(0, DateTime.Now.Millisecond) % 7;
    }
    public override void Update()
    {
        base.Update();
        RotateRenderers();
    }
    public void RotateRenderers()
    {
        rotation = rotation + rotationRate;
        if (rotation > 180f)
        {
            rotation = 180f - rotation;
            axisCounter = (axisCounter + 1) % 7;
        }
        if (rendererWhite != null)
        {
            rendererWhite.transform.eulerAngles = getAngleVector(rotation, (Axis)(axisCounter + 1));
        }
        if (rendererColorized != null)
        {
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
            if (piece.JustTeleported)
            {
                piece.JustTeleported = false;
            }
            else
            {
                Cell targetCell = target.GetComponent<GamePiece>().cell;
                if (targetCell.pieces.Any(p => p is Teleport)) piece.JustTeleported = true;
                piece.TeleportTo(targetCell);
            }
        }
        return base.onOccupy(piece);
    }
}

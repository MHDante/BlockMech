 using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
public class Switch : GamePiece, Triggerable
{
    public override bool isSolid { get { return false; } set { } }
    public override bool isPushable { get { return false; } set { } }
    public bool IsTriggered { get { return flipped; } }
    private bool flipped = false;
    private bool flipping = false;
    private float flipRate = 5f, fliprotation = 0f;
    public override void Start()
    {
        base.Start();
    }
    public override void Update()
    {
        base.Update();
        if (flipping)
        {
            int sign = !flipped ? -1 : 1;
            fliprotation = fliprotation + flipRate * sign;
            if (flipped && fliprotation > 180)
            {
                fliprotation = 180;
                flipping = false;
            }
            else if (!flipped && fliprotation < 0)
            {
                fliprotation = 0;
                flipping = false;
            }
            transform.eulerAngles = getAngleVector(fliprotation, Axis.Xaxis);

            if (fliprotation > 90) transform.eulerAngles += new Vector3(0, 180, 0);
            
        }
    }
    public void StartFlip()
    {
        flipping = true;
        flipped = !flipped;
        RoomManager.roomManager.RefreshColorFamily(colorslot);
        SetColorSlot(colorslot);
    }
    public override void onDeOccupy(GamePiece piece)
    {
        base.onDeOccupy(piece);
    }
    public override bool onOccupy(GamePiece piece)
    {
        if (piece is Player)
        {
            StartFlip();
        }
        return base.onOccupy(piece);
    }

}

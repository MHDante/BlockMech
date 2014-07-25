 using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
public class Switch : GamePiece, Triggerable
{
    public override bool isSolid { get { return false; } set { } }
    public override bool isPushable { get { return false; } set { } }
    public bool IsTriggered 
    { 
        get { return _IsTriggered; } 
        set 
        {
            bool changing = _IsTriggered != value;
            _IsTriggered = value;
            if (changing)
            {
                flipping = true;
                if (RoomManager.roomManager) RoomManager.roomManager.RefreshColorFamily(colorslot);
                SetColorSlot(colorslot);
            }
        }
    }
    [SerializeBlockIt]
    public bool StartsTriggered = false;
    private bool _IsTriggered;
    private bool flipping = false;
    private float flipRate = 5f, fliprotation = 0f;
    protected override void OnValidate()
    {
        base.OnValidate();
        IsTriggered = _IsTriggered;
    }
    public override void Start()
    {
        base.Start();
        IsTriggered = StartsTriggered;
    }
    public override void Update()
    {
        base.Update();
        if (flipping)
        {
            int sign = !IsTriggered ? -1 : 1;
            fliprotation = fliprotation + flipRate * sign;
            if (IsTriggered && fliprotation > 180)
            {
                fliprotation = 180;
                flipping = false;
            }
            else if (!IsTriggered && fliprotation < 0)
            {
                fliprotation = 0;
                flipping = false;
            }
            transform.eulerAngles = getAngleVector(fliprotation, Axis.Xaxis);
            if (fliprotation > 90) transform.eulerAngles += new Vector3(0, 180, 0);
        }
    }
    public override void onDeOccupy(GamePiece piece)
    {
        base.onDeOccupy(piece);
    }
    public override bool onOccupy(GamePiece piece)
    {
        if (piece is Player)
        {
            IsTriggered = !IsTriggered; 
        }
        return base.onOccupy(piece);
    }

}

using UnityEngine;
using System.Collections;
using System;
public class Keyhole : GamePiece
{
    private bool _isSolid = true;
    public override bool isSolid { get { return _isSolid; } set { _isSolid = value; } }
    public override bool isPushable { get { return false; } set { } }
    private bool _active = true;
    public bool active { get { return _active; } set { gameObject.SetActive(value); _isSolid = value; _active = value; } }

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

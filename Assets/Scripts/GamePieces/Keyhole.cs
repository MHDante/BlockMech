using UnityEngine;
using System.Collections;
using System;
using System.Linq;
public class Keyhole : GamePiece
{
    private bool _isSolid = true;
    public override bool isSolid { get { return _isSolid; } set { _isSolid = value; } }
    public override bool isPushable { get { return false; } set { } }
    private bool _locked = true;
    public bool locked { get { return _locked; } set { gameObject.SetActive(value); _isSolid = value; _locked = value; } }

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
    public bool TryOpen(Player player)
    {
        foreach(Key key in player.keys.ToList())
        {
            if (key.colorslot == colorslot)
            {
                player.keys.Remove(key);
                cell.pieces.Remove(this);
                gameObject.SetActive(false);
                return true;
            }
        }
        return false;
    }

}

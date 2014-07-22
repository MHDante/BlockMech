using UnityEngine;
using System.Collections;
using System;
public class Key : GamePiece
{
    public override bool isSolid { get { return false; } set { } }
    public override bool isPushable { get { return false; } set { } }

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
        if (piece is Player)
        {
            Player player = (Player)piece;
            if (player.keys.Contains(this))
            {
                Debug.Log("Player already has key.");
            }
            else
            {
                player.keys.Add(this);
                cell.pieces.Remove(this);
                gameObject.SetActive(false);
            }
            
        }
        return base.onOccupy(piece);

    }

}

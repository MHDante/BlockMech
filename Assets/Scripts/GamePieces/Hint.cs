using UnityEngine;
using System.Collections;

public class Hint : GamePiece {

    public override bool isSolid { get { return false; } set { } }
    public override bool isPushable { get { return false; } set { } }
    [SerializeBlockIt]
    public bool persistent = false;
    [SerializeBlockIt]
    public bool oneShot = false;
    [SerializeBlockIt]
    public string hint;
    private bool consumed = false;
    public override void Awake()
    {
        base.Awake();
        if (Application.isPlaying) foreach (var sr in GetComponentsInChildren<SpriteRenderer>(true)) sr.enabled = false;
    }
    public override bool onOccupy(GamePiece piece)
    {
        if (piece is Player)
        {
            if (!oneShot || !consumed)
            {
                MetaData.instance.UpdateText(hint, colorslot);
            }
        }
        return base.onOccupy(piece);
    }
    public override void onDeOccupy(GamePiece piece)
    {
        base.onDeOccupy(piece);
        if (piece is Player)
        {
            if (!persistent &&(!oneShot || !consumed))
            {
                MetaData.instance.ResetText();
                consumed = true;
            }
        }
    }
}

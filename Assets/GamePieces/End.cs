using UnityEngine;
using System.Collections;
using System;
public class End : GamePiece
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
        if (Application.isPlaying)
        {
            try
            {
                GameManager.instance.totalSteps += Player.steps;
                GameManager.instance.levelSteps = Player.steps;
                GameManager.instance.selState = GameState.ResultsScreen;
            }
            catch (NullReferenceException e)
            {
                Debug.LogWarning("<color=green>Level complete!</color> To see additional functionality, access via the Scene Selector");
            }
        }
        return base.onOccupy(piece);
    }

}

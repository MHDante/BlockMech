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
                GameManager.instance.totalSteps += Player.stats.steps;
                GameManager.instance.levelSteps = Player.stats.steps;
                GameManager.instance.selState = GameState.ResultsScreen;
                Debug.LogWarning("<color=green>Level complete!</color> Steps: " + Player.stats.steps + ". Time: " + Player.stats.time);
            }
            catch (NullReferenceException e)
            {
                Debug.LogWarning("<color=green>Level complete!</color> To see additional functionality, access via the Scene Selector. Steps: " + Player.stats.steps + ". Time: " + Player.stats.time);
            }
        }
        return base.onOccupy(piece);
    }

}

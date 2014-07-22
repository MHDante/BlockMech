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
            if (GameManager.instance != null )
            //try
            {
                Player.stats.Stop();
                GameManager.instance.latestStats = Player.stats;

                GameManager.instance.totalSteps += Player.stats.steps;  //TODO: to be depreciated.
                GameManager.instance.levelSteps = Player.stats.steps;
                GameManager.instance.selState = GameState.ResultsScreen;
                //GameManager.instance.latestStats = Player.stats;
                Debug.Log("<color=green>Level complete!</color> <color=teal>Steps: " + Player.stats.steps + ". Time: " + Player.stats.TimeFormatted() + ".</color>");
            }
            else 
            //catch (NullReferenceException e)
            {
                Debug.Log("<color=green>Level complete!</color> To see additional functionality, access via the <color=magenta>Scene Selector</color>. <color=teal>Steps: " + Player.stats.steps + ". Time: " + Player.stats.TimeFormatted() + ".</color>");
            }
        }
        return base.onOccupy(piece);
    }

}

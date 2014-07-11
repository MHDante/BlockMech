﻿using UnityEngine;
using System.Collections;

public class Author : MonoBehaviour
{
    #region swipe fields
    private float minMovement = 20.0f;
    private bool sendUpMessage = true;
    private bool sendDownMessage = true;
    private bool sendLeftMessage = true;
    private bool sendRightMessage = true;
    private GameObject MessageTarget = null;
    private Vector2 StartPos;
    private int SwipeID = -1;
    #endregion

    public enum difficulty_setting { Easy, Normal, Hard, Nightmare, Battletoads }

    public string author;
    public string levelName;
    public difficulty_setting difficulty;
    public bool usesLock;
    public bool usesBlock;
    public bool usesSwitch;
    public bool usesButton;
    public bool usesTrap;
    public bool usesTeleporter;


    void Awake()
    {
        
    }

    void Update()
    {

        //touch detection code, doesn't belong here :'( 
        if (MessageTarget == null)
            MessageTarget = RoomManager.roomManager.player.gameObject;
        foreach (var T in Input.touches)
        {
            var P = T.position;
            if (T.phase == TouchPhase.Began && SwipeID == -1)
            {
                SwipeID = T.fingerId;
                StartPos = P;
            }
            else if (T.fingerId == SwipeID)
            {
                var delta = P - StartPos;
                if (T.phase == TouchPhase.Moved && delta.magnitude > minMovement)
                {
                    SwipeID = -1;
                    if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
                    {
                        if (sendRightMessage && delta.x > 0)
                            MessageTarget.SendMessage("OnSwipeRight", SendMessageOptions.DontRequireReceiver);
                        else if (sendLeftMessage && delta.x < 0)
                            MessageTarget.SendMessage("OnSwipeLeft", SendMessageOptions.DontRequireReceiver);
                    }
                    else
                    {
                        if (sendUpMessage && delta.y > 0)
                            MessageTarget.SendMessage("OnSwipeUp", SendMessageOptions.DontRequireReceiver);
                        else if (sendDownMessage && delta.y < 0)
                            MessageTarget.SendMessage("OnSwipeDown", SendMessageOptions.DontRequireReceiver);
                    }
                }
                else if (T.phase == TouchPhase.Canceled || T.phase == TouchPhase.Ended)
                {
                    SwipeID = -1;
                    MessageTarget.SendMessage("OnTap", SendMessageOptions.DontRequireReceiver);
                }
            }
        }
    }   
}

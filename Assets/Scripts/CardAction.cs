using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ActionType
{
    CREATED,
    DRAW,
    PLAY,
    REMOVE,
    SELL,
    HOLDING,
    STACK,
    DROP
}

[System.Serializable]
public class CardAction
{
    public ActionType action;
    public AudioClip[] sound;
    public GameObject[] vfx;
}

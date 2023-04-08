using UnityEngine;

public enum ActionType
{
    CARD_CREATE,
    CARD_REMOVE,
    CARD_SELL,
    CARD_HOLDING,
    CARD_STACK,
    CARD_DROP,
    
    ACTION_ADD_TO_SPELL_QUEUE,
    ACTION_REMOVE_FROM_SPELL_QUEUE,
    ACTION_BUY_CARD_PACK,
    
    UI_ELEMENT_CLICKED,

    ENEMY_HIT,
    ENEMY_DIE,
    ENEMY_GROUP_SPAWN,

    SPELL_CASTING,
    SPELL_FIRED,
    SPELL_HIT
}

[System.Serializable]
public class CardAction
{
    public ActionType action;
    public GameObject[] sfx;
    public GameObject[] vfx;
    public Vector3 offset;
}

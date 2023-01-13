using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SlamJam2023/CardData", fileName = "new CardData")]
public class CardData : ScriptableObject
{
    public Material cardImage;
    public Material cardBackground;
    [TextArea]
    public string description;
    public CardAction[] actions;
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SlamJam2023/CardData", fileName = "new CardData")]
public class CardData : ScriptableObject
{
    public bool isSpell;
    public bool cantPutInTomb;
    public int spellDamage;
    public Material cardImage;
    public Material cardBackground;
    [TextArea]
    public string description;
    public int sellCost;
    public CardAction[] actions;
    public GameObject spellPrefab;
}

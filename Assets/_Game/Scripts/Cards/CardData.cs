using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System;

public class CardData : ScriptableObject
{
    [Title("Inspector References")]
    public bool autonomousAbilities;
    public List<AbilityData> abilities;

    [Title("Inspector Variables")]
    public CardStats cardStats;
    public new string name;
    [TextArea]
    public string description;
    public Texture2D cardImage;
    public Texture2D cardBackground;
    public int xSize = 1;
    public int ySize = 1;
}

[Serializable]
public struct CardStats
{
    public int health;
    public int attack;
}
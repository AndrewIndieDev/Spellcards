using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;

public class CardData : ScriptableObject
{
    [Title("Inspector References")]
    public bool autonomousAbilities;
    public List<AbilityData> abilities;

    [Title("Inspector Variables")]
    public new string name;
    [TextArea]
    public string description;
    public Texture2D cardImage;
    public Texture2D cardBackground;
    public int rewardAmount;
    public int xSize = 1;
    public int ySize = 1;
}
using UnityEngine;
using Sirenix.OdinInspector;

public class CardData : ScriptableObject
{
    [Title("Inspector References")]
    public GameObject spellPrefab;

    [Title("Inspector Variables")]
    public string cardName;
    [TextArea]
    public string description;
    public Texture2D cardImage;
    public Texture2D cardBackground;
    public int rewardAmount;
    public int xSize = 1;
    public int ySize = 1;
}
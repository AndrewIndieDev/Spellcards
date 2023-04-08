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
    public Sprite cardImage;
    public Sprite cardBackground;
    public int rewardAmount;
}
using UnityEngine;

[CreateAssetMenu(menuName = "CardData/Structure", fileName = "New Structure")]
public class StructureCard : CardData
{
    public override void OnHit(CardContainer whoHiteMe)
    {
        whoHiteMe?.OnHit(cardStats.attack);
    }
}

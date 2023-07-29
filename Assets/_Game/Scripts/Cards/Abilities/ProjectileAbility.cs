using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "New Projectile Ability", menuName = "Abilities/Projectile")]
public class ProjectileAbility : AbilityAction
{
    private GridManager Grid => GridManager.Instance;

    [Title("Ability Specific Variables")]
    [SerializeField] private GameObject p_Projectile;
    [SerializeField] private GameObject p_ImpactVFX;
    [SerializeField] private Ease r_Ease;

    private Tween currentTween;

    public override float Execute(CardAbility caller)
    {
        GameObject go = Instantiate(p_Projectile, caller.CardContrainer.Collision.Position, Quaternion.identity);
        currentTween = go.transform.DOMove(Grid.SelectionPositionWorld, millisecondsTillComplete / 1000f).SetEase(r_Ease);
        currentTween.onComplete += () =>
        {
            CardContainer found = Grid.GetPlaceableAtPosition(Grid.GetGridCoordsAtWorldPosition(go.transform.position), EGridCellOccupiedFlags.Card) as CardContainer;
            if (found != null)
                found.OnHit(caller.CardContrainer.CardData.cardStats.attack);
            if (p_ImpactVFX != null)
                Instantiate(p_ImpactVFX, go.transform.position, Quaternion.identity);
            Destroy(go);
            caller.CardContrainer.OnKill();
        };
        return millisecondsTillComplete;
    }

    public override void Kill()
    {
        
    }
}

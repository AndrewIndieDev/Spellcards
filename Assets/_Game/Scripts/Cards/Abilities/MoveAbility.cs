using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "New Movement Ability", menuName = "Abilities/Movement")]
public class MoveAbility : AbilityAction
{
    [Title("Ability Specific Variables")]
    [SerializeField] private int moveX;
    [SerializeField] private int moveY;

    private GridManager Grid => GridManager.Instance;

    public override float Execute(CardAbility caller)
    {
        Vector2Int newPos = caller.CardContrainer.GridPosition + new Vector2Int(moveX, moveY);
        caller.CardContrainer.MoveWithVisualDelay(Grid.GetGridCellCenterPosition(newPos));
        return 1f / millisecondsTillComplete;
    }

    public override void Kill()
    {
        throw new System.NotImplementedException();
    }
}

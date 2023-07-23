using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "New Movement Ability", menuName = "Abilities/Movement")]
public class FindAbility : AbilityAction
{
    [Title("Ability Specific Variables")]
    [SerializeField] private EGridCellOccupiedFlags findType;
    [SerializeField] private int searchRangeInGridCells;

    private GridManager Grid => GridManager.Instance;

    public override float Execute(CardAbility caller)
    {
        Vector2Int pos = caller.CardContrainer.GridPosition;
        //Vector2Int targetPos = Grid.FindNearestCellOfType(pos, findType, searchRangeInGridCells);
        return millisecondsTillComplete / 1000f;
    }

    public override void Kill()
    {
        throw new System.NotImplementedException();
    }
}

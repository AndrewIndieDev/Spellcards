using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEditor.Playables;
using UnityEngine;

public class CardAbility : MonoBehaviour
{
    private GridManager Grid => GridManager.Instance;

    [Title("Inspector References")]
    [SerializeField] private CardContainer r_CardContainer;

    public CardContainer CardContrainer { get { return r_CardContainer; } }

    private List<AbilityData> r_Abilities => r_CardContainer.CardData.abilities;

    #region Unity Methods

    #endregion

    #region Public Methods
    /// <summary>
    /// Executes an ability.
    /// </summary>
    public void Execute(AbilityData data)
    {
        if (data == null)
            return;
        if (!data.ExecuteInOrder)
        {
            foreach (var ability in data.Actions)
            {
                ability.Execute(this);
            }
            return;
        }
        StartCoroutine(ExecuteInOrder(data.Actions));
    }
    /// <summary>
    /// Get's a random ability and returns it.
    /// </summary>
    /// <returns>An ability.</returns>
    public AbilityData GetRandomAbility()
    {
        if (r_Abilities == null || r_Abilities.Count <= 0)
            return null;

        if (Utilities.GetRandomNumber(0, 100) < 50)
            return r_Abilities[0];
        return r_Abilities[Random.Range(0, r_Abilities.Count)];
    }
    /// <summary>
    /// Plays random abilities when it can.
    /// </summary>
    public void ExecuteAutonomy()
    {
        StartCoroutine(ExecuteAutonomousAbilities());
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Executes the ability actions in sequence.
    /// </summary>
    /// <param name="actions">List of actions.</param>
    /// <returns>null</returns>
    private IEnumerator ExecuteInOrder(List<AbilityAction> actions)
    {
        foreach (var action in actions)
            yield return new WaitForSeconds(action.Execute(this));
    }
    /// <summary>
    /// Executes the ability actions in sequence, autonomously.
    /// </summary>
    /// <param name="actions">List of actions.</param>
    /// <returns>null</returns>
    private IEnumerator ExecuteAutonomousAbilities()
    {
        while (true) // add check for game state
        {
            var ability = GetRandomAbility();
            if (ability == null)
                break;

            float randTime = (float)Utilities.GetRandomNumber(200, 400) / 100f;
            r_CardContainer.Timer.Run(randTime);
            yield return new WaitForSeconds(randTime);

            if (ability == r_Abilities[0] || r_CardContainer.GridPosition.y == 0)
            {
                IPlaceable found = Grid.GetPlaceableAtPosition(r_CardContainer.GridPosition - new Vector2Int(0, 1), EGridCellOccupiedFlags.Card);
                if (found != null && !(found as CardContainer).IsEnemy)
                {
                    CardContainer toAttack = found as CardContainer;
                    toAttack.OnHit(r_CardContainer.CardData.cardStats.attack, r_CardContainer);
                    continue;
                }
                else if (r_CardContainer.GridPosition.y == 0)
                    HealthBar.Instance.OnHit(r_CardContainer.CardData.cardStats.attack);
            }
            if (ability.ExecuteInOrder)
            {
                foreach (var action in ability.Actions)
                    yield return new WaitForSeconds(action.Execute(this));
            }
            else
            {
                foreach (var action in ability.Actions)
                    action.Execute(this);
            }
        }
    }
    #endregion
}

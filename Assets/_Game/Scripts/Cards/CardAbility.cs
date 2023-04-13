using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Playables;
using UnityEngine;

public class CardAbility : MonoBehaviour
{
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
        if (!data.PlayInSequence)
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
        {
            yield return new WaitForSeconds(action.Execute(this));
        }
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
            if (ability.PlayInSequence)
            {
                foreach (var action in ability.Actions)
                {
                    float seconds = action.Execute(this);
                    yield return new WaitForSeconds(seconds);
                }
            }
            else
            {
                foreach (var action in ability.Actions)
                {
                    action.Execute(this);
                }
            }
            yield return new WaitForSeconds(1f);
        }
    }
    #endregion
}

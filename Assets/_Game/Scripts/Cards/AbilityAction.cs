using UnityEngine;
using System;
using Sirenix.OdinInspector;

public enum AbilityType
{
    RANGED
}

public enum AbilityTarget
{
    GROUND,
    CARD
}

[Serializable]
public class AbilityAction : ScriptableObject
{
    [Title("Inspector Variables")]
    [SerializeField] protected new string name;
    [SerializeField] protected int millisecondsTillComplete;

    /// <summary>
    /// Executes the actions to be played. Returns the time it takes to complete the action.
    /// </summary>
    /// <returns>Time till action complete.</returns>
    public virtual float Execute(CardAbility caller)
    {
        return 0f;
    }
    /// <summary>
    /// Kills the action.
    /// </summary>
    public virtual void Kill()
    {

    }
}
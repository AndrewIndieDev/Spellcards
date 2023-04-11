using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New AbilityData", menuName = "Abilities/Data")]
public class AbilityData : ScriptableObject
{
    [SerializeField] private bool playInSequence;
    public List<AbilityAction> abilities;

    public bool PlayInSequence { get { return playInSequence; } }
    public List<AbilityAction> Abilities { get { return abilities; } }
}
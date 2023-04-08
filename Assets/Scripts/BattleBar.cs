using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class BattleBar : MonoBehaviour
{
    [Title("Inspector Variables")]
    [SerializeField] private float unitsPerMinute;
    [SerializeField] private int shownEvents;
    [SerializeField] private Transform barEnd;
    [SerializeField] private List<Transform> eventObjects = new();
    [SerializeField] private List<CardData> eventData = new();

    [Title("Read Only Variables")]
    [ReadOnly][SerializeField] private float currentTimeInSeconds;

    #region Unity Methods
    private void Update()
    {
        currentTimeInSeconds += Time.deltaTime;
        UpdateBar();
    }
    #endregion

    #region Public Methods

    #endregion

    #region Private Methods
    private void UpdateBar()
    {
        
    }
    #endregion
}
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

public class BattleBar : MonoBehaviour
{
    [Title("Inspector Variables")]
    [Range(1, 10)]
    [SerializeField] private int shownEvents;
    [SerializeField] private float maxTimeOnBar;
    [SerializeField] private Transform barStart;
    [SerializeField] private Transform barEnd;

    [Title("Lists")]
    [SerializeField] private List<Transform> eventObjects = new();
    [SerializeField] private List<EventData> eventData = new();

    [Title("Read Only Variables")]
    [ReadOnly][SerializeField] private float currentTime;

    #region Unity Methods
    private void Update()
    {
        currentTime += Time.deltaTime;
        if (eventData.Count > 0)
            UpdateBar();
    }
    #endregion

    #region Public Methods

    #endregion

    #region Private Methods
    private void UpdateBar()
    {
        for (int i = 0; i < eventObjects.Count; i++)
        {
            eventObjects[i].gameObject.SetActive(i < shownEvents && i < eventData.Count);
        }

        for (int i = 0; i < eventData.Count; i++)
        {
            float pos = Mathf.InverseLerp(0, maxTimeOnBar, eventData[i].timeToSpawn - currentTime);
            eventObjects[i].transform.localPosition = Vector3.Lerp(barStart.localPosition, barEnd.localPosition, 1f - pos);

            if (eventObjects[i].transform.localPosition.z >= 1)
            {
                SpawningManager.Instance.SpawnEnemy(eventData[i].cardToSpawn);
                eventData.RemoveAt(i);
            }
        }
    }
    #endregion
}

[Serializable]
public struct EventData
{
    public int timeToSpawn;
    public CardData cardToSpawn;
}
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

public class BattleBar : MonoBehaviour
{
    private SpawningManager Spawner => SpawningManager.Instance;

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
    /// <summary>
    /// Adds an event to the list of events.
    /// </summary>
    /// <param name="spawnAtTime">Time (in seconds) that you want the event to trigger.</param>
    /// <param name="cardToSpawn">Data that you want to spawn.</param>
    public void AddEvent(int spawnAtTime, CardData cardToSpawn)
    {
        eventData.Add(new EventData(spawnAtTime, cardToSpawn));
    }
    /// <summary>
    /// Removes an event at the given index.
    /// </summary>
    /// <param name="index">Index of the event.</param>
    public void RemoveEventAtIndex(int index)
    {
        eventData.RemoveAt(index);
    }
    /// <summary>
    /// Removes the closest event with the given card type.
    /// </summary>
    /// <param name="data">Data type of the event.</param>
    public void RemoveNextEventOfCardType(CardData data)
    {
        for (int i = 0; i < eventData.Count; i++)
        {
            if (eventData[i].cardToSpawn == data)
            {
                eventData.RemoveAt(i);
                return;
            }
        }
    }
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
                Spawner.SpawnCard(eventData[i].cardToSpawn);
                RemoveEventAtIndex(i);
            }
        }
    }
    #endregion

    [Serializable]
    private struct EventData
    {
        public int timeToSpawn;
        public CardData cardToSpawn;

        public EventData(int timeToSpawn, CardData cardToSpawn)
        {
            this.timeToSpawn = timeToSpawn;
            this.cardToSpawn = cardToSpawn;
        }
    }
}
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class CardTimerManager : MonoBehaviour
{
    public static CardTimerManager Instance;

    [Title("Read Only Variables")]
    [ReadOnly][SerializeField] private List<CardTimer> active = new();

    #region Public Methods
    /// <summary>
    /// Adds a timer to the list of running timers.
    /// </summary>
    /// <param name="timer">Timer to add to the running timers list</param>
    public void Run(CardTimer timer)
    {
        Utilities.AddUnique(active, timer);
    }
    /// <summary>
    /// Removes a timer from the active timers list.
    /// </summary>
    /// <param name="timer"></param>
    public void Pause(CardTimer timer)
    {
        Utilities.RemoveUnique(active, timer);
    }
    #endregion

    #region Unity Methods
    private void Awake()
    {
        if (Instance != null)
        {
            Dbug.Instance.LogInstanceAlreadyExists(this);
            return;
        }
        Instance = this;
    }

    private void Update()
    {
        for (int i = active.Count - 1; i >= 0; i--)
        {
            if (active[i] == null)
            {
                active.RemoveAt(i);
                continue;
            }
            CardTimer timer = active[i];
            timer.UpdateTime(Time.deltaTime);
        }
    }
    #endregion
}

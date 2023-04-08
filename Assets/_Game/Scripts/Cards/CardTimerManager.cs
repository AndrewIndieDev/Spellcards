using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class CardTimerManager : MonoBehaviour
{
    public static CardTimerManager Instance;

    [Title("Read Only Variables")]
    [ReadOnly][SerializeField] private List<Timer> active = new();

    #region Public Methods
    /// <summary>
    /// Adds a timer to the list of running timers.
    /// </summary>
    /// <param name="timer">Timer to add to the running timers list</param>
    public void Run(Timer timer)
    {
        Utilities.AddUnique(active, timer);
    }
    /// <summary>
    /// Removes a timer from the active timers list.
    /// </summary>
    /// <param name="timer"></param>
    public void Pause(Timer timer)
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
        foreach (var timer in active)
        {
            timer.timeInSeconds -= Time.deltaTime;
            if (timer.timeInSeconds <= 0.0f)
                timer.Finished();
        }
    }
    #endregion
}

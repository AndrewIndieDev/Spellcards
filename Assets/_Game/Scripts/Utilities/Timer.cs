using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

public class Timer : MonoBehaviour
{
    public CardTimerManager TimerManager { get { return CardTimerManager.Instance; } }

    [Title("Inspector References")]
    [SerializeField] private Transform r_Percentage;
    [SerializeField] private GameObject r_TimerVisual;
    [SerializeField] private UnityEvent r_TimerActions;

    [Title("Read Only Variables")]
    [ReadOnly] public float timeInSeconds;

    #region Public Methods
    /// <summary>
    /// Starts the timer with an amount of seconds till it finishes.
    /// </summary>
    /// <param name="time">Seconds till the timer finishes.</param>
    public void Run(float time)
    {
        timeInSeconds = time;
        r_TimerVisual.SetActive(true);
        TimerManager.Run(this);
    }
    /// <summary>
    /// Pauses the current timer.
    /// </summary>
    public void Pause()
    {
        TimerManager.Pause(this);
    }
    /// <summary>
    /// Resumes the current timer.
    /// </summary>
    public void Resume()
    {
        TimerManager.Run(this);
    }
    /// <summary>
    /// Stops the timer and disables it.
    /// </summary>
    public void Clear()
    {
        r_TimerVisual.SetActive(false);
        Pause();
    }
    /// <summary>
    /// Disables the timer, then sends a notification that this timer has finished.
    /// </summary>
    public void Finished()
    {
        Clear();
        // Tell container that it's finished?
    }
    #endregion
}
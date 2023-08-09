using Sirenix.OdinInspector;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CardTimer : MonoBehaviour
{
    public CardTimerManager TimerManager { get { return CardTimerManager.Instance; } }

    [Title("Inspector References")]
    [SerializeField] private Transform r_Percentage;
    [SerializeField] private GameObject r_TimerVisual;
    [SerializeField] private MeshRenderer r_TimerMesh;
    [SerializeField] private UnityEvent r_TimerActions;

    [Title("Read Only Variables")]
    [ReadOnly] private float r_Duration;
    [ReadOnly] public float timeInSeconds;

    private Action cb_OnFinish;

    #region Public Methods
    /// <summary>
    /// Starts the timer with an amount of seconds till it finishes.
    /// </summary>
    /// <param name="time">Seconds till the timer finishes.</param>
    public void Run(float time, Action callback = null)
    {
        cb_OnFinish = callback;
        r_Duration = time;
        timeInSeconds = time;
        r_TimerVisual.SetActive(true);
        TimerManager.Run(this);
    }
    /// <summary>
    /// Updates the current time by deltaTime and updates the timer mesh.
    /// </summary>
    /// <param name="deltaTime">deltaTime passed in.</param>
    public void UpdateTime(float deltaTime)
    {
        timeInSeconds -= deltaTime;
        r_TimerMesh.material.SetFloat("_Timer", timeInSeconds / r_Duration);

        if (timeInSeconds <= 0.0f)
            Finished();
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
    public void Cancel()
    {
        r_TimerMesh.material.SetFloat("_Timer", 0.0f);
        r_TimerVisual.SetActive(false);
        cb_OnFinish = null;
        Pause();
    }
    /// <summary>
    /// Disables the timer, then sends a notification that this timer has finished.
    /// </summary>
    public void Finished()
    {
        cb_OnFinish?.Invoke();
        Cancel();
    }
    #endregion
}
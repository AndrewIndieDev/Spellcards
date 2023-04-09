using UnityEngine;
using Sirenix.OdinInspector;
using System;

public class CardContainer : MonoBehaviour, IPlaceable
{
    private GridManager Grid => GridManager.Instance;

    [Title("Inspector References")]
    //[SerializeField] private CardState r_State;
    [SerializeField] private CardVisuals r_Visuals;
    [SerializeField] private CardCollision r_Collision;
    [SerializeField] private Timer r_Timer;

    //public CardState State { get { return r_State; } }
    public CardVisuals Visuals { get { return r_Visuals; } }
    public CardCollision Collision { get { return r_Collision; } }
    public Timer Timer { get { return r_Timer; } }

    [Title("Inspector Variables")]
    public float DEFAULT_TWEEN_TIME;

    [Title("Read Only Variables")]
    [ReadOnly][SerializeField] private CardData r_Data;

    public CardData CardData { get { return r_Data; } }

    #region Interface Methods
    /// <summary>
    /// How big the card is (1x1, 2x2, etc.). This is used for the grid system.
    /// </summary>
    public Vector2Int Size => new Vector2Int(r_Data.xSize, r_Data.ySize);
    /// <summary>
    /// This is the grid position that the card is currently on.
    /// </summary>
    public Vector2Int GridPosition => Grid.GetGridCoordsAtWorldPosition(Collision.Position);
    /// <summary>
    /// Called when the card is placed on the grid.
    /// </summary>
    public void OnPlace()
    {
        
    }
    /// <summary>
    /// Called when the card is removed from the grid.
    /// </summary>
    public void OnKill()
    {
        
    }
    /// <summary>
    /// Called when the card is interacted with.
    /// </summary>
    public void OnInteract()
    {
        Dbug.Instance.Log($"Interacted with {r_Data.cardName}!");
    }
    #endregion

    #region Unity Methods
    private void Start()
    {
        GridManager.onSelectionPositionChanged += OnSelectionPositionChanged;

        UpdateAll();
    }
    void OnDestroy()
    {
        GridManager.onSelectionPositionChanged -= OnSelectionPositionChanged;
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// Updates the current card with the given data.
    /// </summary>
    /// <param name="data">The CardData for this card</param>
    public void SetData(CardData data)
    {
        r_Data = data;
        r_Visuals.Set(r_Data);
    }
    /// <summary>
    /// Used to send a notification to components to update themselves.
    /// </summary>
    public void UpdateAll()
    {
        r_Visuals.Set(r_Data);
        //r_State.CheckForUpdates();
        r_Collision.CheckForUpdates();
    }
    /// <summary>
    /// This is the callback method for the attached Timer's Unity Event.
    /// </summary>
    public void TimerFinished()
    {
        // Do something once the timer on the Timer gameObject has finished.
    }
    /// <summary>
    /// Sets the parent for the Container and resets local position.
    /// </summary>
    /// <param name="newParent">New parent for the Container.</param>
    public void SetParent(Transform newParent)
    {
        transform.parent = newParent;
        ResetLocalPosition();
    }
    /// <summary>
    /// Sells the current card and adds the sell cost to the GameManager's currency.
    /// </summary>
    public void Sell()
    {
        GameManager.Instance.AddCurrency(CardData.rewardAmount);
        Destroy(gameObject);
    }
    /// <summary>
    /// Tweens the card to the given position.
    /// </summary>
    /// <param name="position">Position to move to.</param>
    public void Move(Vector3 position)
    {
        Collision.Move(position);
        Visuals.Move(position);
    }
    /// <summary>
    /// Instantly moves the the given position.
    /// </summary>
    /// <param name="position">Position to move to.</param>
    public void MoveInstant(Vector3 position)
    {
        Collision.MoveInstant(position);
        Visuals.MoveInstant(position);
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Resets local transform. Used mainly to attach cards.
    /// </summary>
    private void ResetLocalPosition()
    {
        transform.localPosition = Vector3.zero;
    }
    /// <summary>
    /// Called when the grid selection position changes.
    /// </summary>
    /// <param name="newPosition">New position of the selection.</param>
    private void OnSelectionPositionChanged(Vector2Int newPosition)
    {
        if (!Collision.PickedUp || !GridManager.Instance.GetIsSelectable(newPosition)) return;

        Visuals.Move(GridManager.Instance.SelectionPositionWorld);
    }
    #endregion
}

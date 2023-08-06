using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngineInternal;

public class CardContainer : MonoBehaviour, IPlaceable, IDamageable
{
    [Title("Inspector References")]
    [SerializeField] private CardVisuals r_Visuals;
    [SerializeField] private CardCollision r_Collision;
    [SerializeField] private CardAbility r_Abilities;
    [SerializeField] private CardTimer r_Timer;

    public GridManager Grid { get { return GridManager.Instance; } }
    public CardVisuals Visuals { get { return r_Visuals; } }
    public CardCollision Collision { get { return r_Collision; } }
    public CardTimer Timer { get { return r_Timer; } }

    [Title("Inspector Variables")]
    public float DEFAULT_TWEEN_TIME;

    [Title("Read Only Variables")]
    [ReadOnly][SerializeField] private CardData r_Data;
    [ReadOnly][SerializeField] private int cardHealth;
    [ReadOnly][SerializeField] private int cardAttack;

    public CardData CardData { get { return r_Data; } }

    public bool IsEnemy => r_Data.GetType() == typeof(EnemyCard);

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
        Grid.UnoccupyGridField(GridPosition);

        CardData toSpawn = r_Data.dropList.RandomElement();
        if (toSpawn != null && Utilities.GetRandomNumber(0, 2) == 0)
            SpawningManager.Instance.SpawnCard(toSpawn, GridPosition);

        Timer.Cancel();

        Destroy(gameObject);
    }
    /// <summary>
    /// Called when the card is interacted with.
    /// </summary>
    public void OnInteract()
    {
        Collision.OnInteract();
    }
    /// <summary>
    /// Called when the card is interacted with to execute an ability.
    /// </summary>
    public void OnExecute()
    {
        if (IsEnemy)
            return;
        r_Abilities.Execute(CardData.abilities[0]);
    }
    /// <summary>
    /// Called when the card has been spawned for the first time.
    /// </summary>
    public void OnSpawn()
    {
        if (IsEnemy)
            r_Abilities.ExecuteAutonomy();
    }
    /// <summary>
    /// Called when something wants to hit this object.
    /// </summary>
    public void OnHit(int amount, CardContainer hitBy = null)
    {
        CardData.OnHit(hitBy);

        cardHealth -= amount;
        if (cardHealth <= 0)
        {
            cardHealth = 0;
            OnKill();
            return;
        }

        Visuals.OnHit();
        Visuals.UpdateHealth(cardHealth);
    }
    #endregion

    #region Unity Methods
    private void Start()
    {
        UpdateAll();
        GridManager.onSelectionGridPositionChanged += OnSelectionGridPositionChanged;
        GameManager.onInteractUp += OnInteractUp;
        
    }
    void OnDestroy()
    {
        GridManager.onSelectionGridPositionChanged -= OnSelectionGridPositionChanged;
        GameManager.onInteractUp -= OnInteractUp;
    }
    #endregion

    #region Callbacks
    /// <summary>
    /// Called when the grid cell selection position changes.
    /// </summary>
    /// <param name="position">New position.</param>
    private void OnSelectionGridPositionChanged(Vector2Int position)
    {
        if (!Collision.PickedUp) return;

        //MoveWithVisualDelay(Grid.SelectionPositionWorld);
        Visuals.Move(Grid.SelectionPositionWorld);
    }
    /// <summary>
    /// Activates when the player releases the interact button.
    /// </summary>
    private void OnInteractUp()
    {
        if (!Collision.PickedUp) return;

        SwapCardPositions();

        Collision.OnInteractUp();
        Visuals.OnInteractUp();
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
        cardHealth = data.cardStats.health;
        cardAttack = data.cardStats.attack;
    }
    /// <summary>
    /// Used to send a notification to components to update themselves.
    /// </summary>
    public void UpdateAll()
    {
        r_Visuals.Set(r_Data);
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
    /// Tweens the card to the given position.
    /// </summary>
    /// <param name="position">Position to move to.</param>
    public void Move(Vector3 position)
    {
        if (!IsGridPositionAcceptable(Grid.GetGridCoordsAtWorldPosition(position)))
            return;

        Collision.Move(position);
        Visuals.Move(position);
    }
    /// <summary>
    /// Instantly moves the the given position.
    /// </summary>
    /// <param name="position">Position to move to.</param>
    public void MoveInstant(Vector3 position)
    {
        if (!IsGridPositionAcceptable(Grid.GetGridCoordsAtWorldPosition(position)))
            return;

        Collision.MoveInstant(position);
        Visuals.MoveInstant(position);
    }
    /// <summary>
    /// Instantly moves the collision to the new position, but visually moves the card to the new position with a delay.
    /// </summary>
    /// <param name="position">Position to move.</param>
    /// <param name="unoccupy">(Default: true) Should the move unoccupy the current grid cell.</param>
    public void MoveWithVisualDelay(Vector3 position)
    {
        if (!IsGridPositionAcceptable(Grid.GetGridCoordsAtWorldPosition(position)))
            return;

        Collision.MoveInstant(position);
        Visuals.Move(position);
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
    /// This checks to see if the grid position given is free of all flags.
    /// </summary>
    /// <param name="position">Grid position to check.</param>
    /// <returns>If grid position is free of all flags.</returns>
    private bool IsGridPositionAcceptable(Vector2Int position)
    {
        return
            Grid.WithinGridPlayArea(position) &&
            Grid.GridPositionFree(position, EGridCellOccupiedFlags.Card);
    }
    /// <summary>
    /// Checks to see if cards need to swap positions and does so.
    /// </summary>
    /// <returns>If cards needed to swap.</returns>
    private void SwapCardPositions()
    {
        IPlaceable placeable = Grid.GetPlaceableAtPosition(Grid.SelectionPositionGrid, EGridCellOccupiedFlags.Card);
        if (placeable == null || placeable == (IPlaceable)this)
            return;

        CardContainer cardAtPosition = (CardContainer)placeable;
        Grid.UnoccupyGridField(cardAtPosition.GridPosition, cardAtPosition);
        Grid.UnoccupyGridField(GridPosition, this);

        cardAtPosition.MoveWithVisualDelay(Collision.Position);
    }
    #endregion
}

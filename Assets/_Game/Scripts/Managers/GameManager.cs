using System.Collections;
using UnityEngine;
using UnityEngine.VFX;
using Sirenix.OdinInspector;
using System;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private GridManager Grid => GridManager.Instance;
    private PlayerInputActions InputActions => InputManager.Instance.InputActions;

    public delegate void OnInteractUp();
    public static event OnInteractUp onInteractUp;

    [Title("Inspector References")]
    [SerializeField] private VisualEffect coins;

    [Title("Inspector Variables")]
    [SerializeField] private float generateGoldEveryXSec;
    [SerializeField] private int currency;
    [SerializeField] private LayerMask table;

    [Title("Read Only Variables")]
    [ReadOnly][SerializeField] private bool playing;

    #region Unity Methods
    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        InputActions.Player.Interact.performed += OnInteract;
        InputActions.Player.Interact.canceled += OnInteract;
        InputActions.Player.Execute.performed += OnExecute;
        InputActions.Player.NavigationMouse.performed += OnNavigationMouse;
        InputActions.Player.NavigationGamepad.performed += OnNavigationGamepad;
    }
    private void OnDestroy()
    {
        InputActions.Player.Interact.performed -= OnInteract;
        InputActions.Player.Interact.canceled -= OnInteract;
        InputActions.Player.Execute.performed -= OnExecute;
        InputActions.Player.NavigationMouse.performed -= OnNavigationMouse;
        InputActions.Player.NavigationGamepad.performed -= OnNavigationGamepad;
    }
    #endregion

    #region Callbacks
    private void OnInteract(InputAction.CallbackContext context)
    {
        if (context.performed)
            Grid.InteractPlaceable(Grid.SelectionPositionGrid, EGridCellOccupiedFlags.Card);
        else if (context.canceled)
            onInteractUp?.Invoke();
    }
    private void OnExecute(InputAction.CallbackContext context)
    {
        Grid.ExecutePlaceable(Grid.SelectionPositionGrid);
    }
    private void OnNavigationMouse(InputAction.CallbackContext context)
    {
        Grid.OnNavigationMouse(context);
    }
    private void OnNavigationGamepad(InputAction.CallbackContext context)
    {
        var vpos = context.ReadValue<Vector2>();
        var newPosition = Grid.SelectionPositionGrid + new Vector2Int(Mathf.RoundToInt(vpos.x), Mathf.RoundToInt(vpos.y));
        Grid.MoveSelection(newPosition);
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// Adds a set amount of currency to your total amount.
    /// </summary>
    /// <param name="amount">Amount to add.</param>
    public void AddCurrency(int amount)
    {
        currency += amount;
        CheckCoinAmount();
    }
    /// <summary>
    /// Removes a set amount of currency from your total amount.
    /// </summary>
    /// <param name="amount">Amount to remove.</param>
    /// <returns>True if you had enough currency, False if you didn't.</returns>
    public bool RemoveCurrency(int amount)
    {
        if (currency - amount < 0)
            return false;
        currency -= amount;
        CheckCoinAmount();
        return true;
    }
    /// <summary>
    /// Starts the game.
    /// </summary>
    public void GameStart()
    {
        if (playing) return;

        AddCurrency(15);
        playing = true;
        StartCoroutine(GoldOverTime());
    }
    /// <summary>
    /// Ends the game.
    /// </summary>
    public void GameEnd()
    {
        if (!playing) return;

        playing = false;
        RemoveCurrency(currency);
    }
    /// <summary>
    /// Quits the game.
    /// </summary>
    public void Quit()
    {
        Application.Quit();
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Used to update the coin visuals.
    /// </summary>
    private void CheckCoinAmount()
    {
        coins.Reinit();
        coins.SetInt("Coin Initialize", currency);
        coins.SendEvent("Initialize");
    }
    /// <summary>
    /// Used to generate gold over time.
    /// </summary>
    private IEnumerator GoldOverTime()
    {
        float cooldown = generateGoldEveryXSec;
        while (playing)
        {
            if (!playing) break;
            cooldown -= Time.deltaTime;
            if (cooldown <= 0)
            {
                AddCurrency(1);
                cooldown = generateGoldEveryXSec;
            }
            yield return null;
        }
    }
    #endregion
}

using System.Collections;
using UnityEngine;
using UnityEngine.VFX;
using Sirenix.OdinInspector;
using System;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private GridManager Grid => GridManager.Instance;
    private PlayerInputActions InputActions => InputManager.Instance.InputActions;

    public delegate void OnInteractUp();
    public static event OnInteractUp onInteractUp;

    [Title("Inspector References")]
    [SerializeField] private AbilityVisual r_AbilityVisual;

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
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
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
        CardContainer card = Grid.GetPlaceableAtPosition(Grid.SelectionPositionGrid, EGridCellOccupiedFlags.Card) as CardContainer;
        if (card == null || card.CardData.abilities.Count <= 0)
            return;
        r_AbilityVisual.Init(card.CardData.abilities[0].abilityStyle, card);
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
    /// Starts the game.
    /// </summary>
    public void GameStart()
    {
        if (playing)
            return;
        playing = true;
    }
    /// <summary>
    /// Ends the game.
    /// </summary>
    public void GameEnd()
    {
        if (!playing)
            return;
        playing = false;
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

    #endregion
}

using System.Collections;
using UnityEngine;
using UnityEngine.VFX;
using Sirenix.OdinInspector;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private GridManager Grid => GridManager.Instance;
    
    public Action OnGameStart;
    public Action OnGameEnd;
    public Action<CardContainer> OnCardPickup;
    public Action OnCardDrop;

    [Title("Inspector References")]
    [SerializeField] private VisualEffect coins;

    [Title("Inspector Variables")]
    [SerializeField] private float generateGoldEveryXSec;
    [SerializeField] private int currency;
    [SerializeField] private LayerMask table;

    [Title("Read Only Variables")]
    [ReadOnly] public Vector3 MousePosition;
    [ReadOnly][SerializeField] private bool playing;

    #region Unity Methods
    private void Awake()
    {
        Instance = this;
    }
    private void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(ray, out RaycastHit hit, 1000f, table);
        MousePosition = hit.point.IgnoreAxis(EAxis.Y, 0.1f);

        if (Input.GetMouseButtonDown(1))
        {
            var position = Grid.GetGridCoordsAtWorldPosition(MousePosition);
            if (position.x < 0 || position.x >= Grid.GridWidth) return;
            if (position.y < 0 || position.y >= Grid.GridHeight) return;

            Grid.InteractWithPlaceable(position);
        }
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

        OnGameStart?.Invoke();

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

        OnGameEnd?.Invoke();

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

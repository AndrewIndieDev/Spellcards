using QFSW.QC;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    private void Awake()
    {
        Instance = this;
    }

    public Card cardPrefab;
    public CardData failedCreationRef;
    public VisualEffect coins;

    public float generateGoldEveryXSec = 3f;
    public int currency = 0;

    public LayerMask table;
    public Vector3 MousePosition;

    public System.Action OnGameStart;
    public System.Action OnGameEnd;

    public bool playing;

    [Command]
    private void SpawnCards(int amount)
    {
        Card card = null;
        CardData[] cards = Resources.LoadAll<CardData>("Cards");
        for (int i = 0; i < amount; i++)
        {
            card = Instantiate(cardPrefab, new Vector3(Random.Range(-0.8f, 0.8f), 0f, Random.Range(-0.3f, 0.3f)), Quaternion.identity);
            card.cardData = cards[i%cards.Length];
        }
    }

    public GameObject SpawnCard(CardData data, Vector3 position)
    {
        Card spawned = Instantiate(cardPrefab, position, Quaternion.identity);
        spawned.cardData = data;
        return spawned.gameObject;
    }

    private void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(ray, out RaycastHit hit, 1000f, table);
        MousePosition = hit.point;
        MousePosition.y += 0.015f;
    }

    [Command]
    public void AddCurrency(int amount)
    {
        currency += amount;
        CheckCoinAmount();
    }

    public bool RemoveCurrency(int amount)
    {
        if (currency - amount < 0)
            return false;
        currency -= amount;
        CheckCoinAmount();
        return true;
    }

    private void CheckCoinAmount()
    {
        coins.Reinit();
        coins.SetInt("Coin Initialize", currency);
        coins.SendEvent("Initialize");
    }

    [Command]
    public void GameStart()
    {
        if (playing) return;

        OnGameStart?.Invoke();

        AddCurrency(15);
        playing = true;
        StartCoroutine(GoldOverTime());
    }

    [Command]
    public void GameEnd()
    {
        if (!playing) return;

        OnGameEnd?.Invoke();

        playing = false;
        RemoveCurrency(currency);
    }

    private IEnumerator GoldOverTime()
    {
        while (playing)
        {
            yield return new WaitForSeconds(generateGoldEveryXSec);
            if (!playing) break;
            AddCurrency(1);
        }
    }
}

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

    public int currency = 0;
    private int oldCurrency;

    public LayerMask table;
    public Vector3 MousePosition;

    private void Start()
    {
        AddCurrency(15);
    }

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
        oldCurrency = currency;
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
}

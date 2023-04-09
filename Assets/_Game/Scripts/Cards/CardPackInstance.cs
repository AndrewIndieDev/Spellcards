using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CardPackInstance : MonoBehaviour
{
    private SpawningManager Spawner => SpawningManager.Instance;
    private GridManager Grid => GridManager.Instance;
    
    public int cardsToSpawn;
    public List<CardData> potentialCards = new();

    public void Open()
    {
        StartCoroutine(SpawnCard());
    }

    private IEnumerator SpawnCard()
    {
        while (cardsToSpawn > 0)
        {
            Spawner.SpawnCard(potentialCards[Random.Range(0, potentialCards.Count)], transform.position);
            cardsToSpawn--;
            yield return new WaitForSeconds(0.01f);
        }
        Destroy(gameObject);
    }
}
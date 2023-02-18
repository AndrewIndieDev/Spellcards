using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CardPackInstance : MonoBehaviour
{
    public int cardsToSpawn;
    public List<CardData> potentialCards = new();
    public Transform radialGuide;

    public void Open()
    {
        StartCoroutine(SpawnCard());
    }

    private IEnumerator SpawnCard()
    {
        float increment = 360f / cardsToSpawn;
        while (cardsToSpawn > 0)
        {
            GameObject spawned = GameManager.Instance.SpawnCard(potentialCards[Random.Range(0, potentialCards.Count)], GameManager.Instance.MousePosition);
            spawned.transform.DOMove(transform.position + radialGuide.forward * 0.15f, 0.1f);
            radialGuide.DORotate(new Vector3(0f, radialGuide.rotation.eulerAngles.y + increment, 0f), 0.005f);
            cardsToSpawn--;
            yield return new WaitForSeconds(0.01f);
        }
        Destroy(gameObject);
    }
}
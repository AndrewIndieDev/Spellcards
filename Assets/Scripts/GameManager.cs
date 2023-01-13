using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    private void Awake()
    {
        Instance = this;
    }

    public Card cardPrefab;

    public LayerMask table;
    public Vector3 MousePosition;

    private void Start()
    {
        Card card = null;
        foreach (CardData data in Resources.LoadAll<CardData>("Cards"))
        {
            card = Instantiate(cardPrefab, new Vector3(Random.Range(-40f, 40f), 0f, Random.Range(-15, 15)), Quaternion.identity);
            card.cardData = data;
        }
    }

    private void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(ray, out RaycastHit hit, 1000f, table);
        MousePosition = hit.point;
        MousePosition.y = 0.8f;
    }
}
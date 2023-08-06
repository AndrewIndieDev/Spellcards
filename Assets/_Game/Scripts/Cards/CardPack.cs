using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class CardPack : MonoBehaviour
{
    private GridManager Grid => GridManager.Instance;
    private GameManager Game => GameManager.Instance;

    //public TMP_Text costPopup;
    public GameObject packPrefab;
    public int cost;

    //private Tween currentTween;
    private GameObject draggingPack;

    //private void Start()
    //{
    //    costPopup.text = cost.ToString();
    //}

    //private void OnMouseEnter()
    //{
    //    if (currentTween != null)
    //        currentTween.Kill();
    //    currentTween = costPopup.DOColor(Color.white, 0.1f);
    //}

    //private void OnMouseExit()
    //{
    //    if (currentTween != null)
    //        currentTween.Kill();
    //    costPopup.DOColor(new Color(1f, 1f, 1f, 0f), 0.1f);
    //}

    private void OnMouseDown()
    {
        //if (GameManager.Instance.RemoveCurrency(cost))
        //    draggingPack = Instantiate(packPrefab, transform.position, Quaternion.identity);
    }

    private void OnMouseDrag()
    {
        if (draggingPack == null) return;

        draggingPack.transform.position = Grid.SelectionPositionWorld;
    }

    private void OnMouseUp()
    {
        if (draggingPack == null) return;

        draggingPack.GetComponent<CardPackInstance>().Open();
        draggingPack = null;
    }
}

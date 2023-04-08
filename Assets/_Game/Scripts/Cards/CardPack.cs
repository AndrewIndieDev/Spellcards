using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class CardPack : MonoBehaviour
{
    public TMP_Text costPopup;
    public GameObject packPrefab;
    public int cost;

    private Tween currentTween;
    private GameObject draggingPack;

    private void Start()
    {
        costPopup.text = cost.ToString();
    }

    private void OnMouseEnter()
    {
        if (currentTween != null)
            currentTween.Kill();
        currentTween = costPopup.DOColor(Color.white, 0.1f);
    }

    private void OnMouseExit()
    {
        if (currentTween != null)
            currentTween.Kill();
        costPopup.DOColor(new Color(1f, 1f, 1f, 0f), 0.1f);
    }

    private void OnMouseDown()
    {
        if (GameManager.Instance.RemoveCurrency(cost))
            draggingPack = Instantiate(packPrefab, transform.position, Quaternion.identity);
    }

    private void OnMouseDrag()
    {
        if (draggingPack == null) return;

        draggingPack.transform.position = GameManager.Instance.MousePosition;
    }

    private void OnMouseUp()
    {
        if (draggingPack == null) return;

        if (Vector3.Distance(draggingPack.transform.position, transform.position) < 0.3f)
            draggingPack.transform.position = transform.position + Vector3.left * 0.3f;

        draggingPack.GetComponent<CardPackInstance>().Open();
        draggingPack = null;
    }
}

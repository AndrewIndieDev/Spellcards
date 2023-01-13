using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;
using TMPro;

public class Card : MonoBehaviour
{
    public CardData cardData;
    public MeshRenderer mesh_CardImage;
    public MeshRenderer mesh_BackgroundImage;
    public TextMeshProUGUI cardName;

    private Tween currentTween;
    private bool pickedUp;
    private Vector3 offset;

    private void Start()
    {
        if (cardData == null) return;
        if (mesh_CardImage)
            mesh_CardImage.material = cardData.cardImage;
        if (mesh_BackgroundImage)
            mesh_BackgroundImage.material = cardData.cardBackground;
        if (cardName)
            cardName.text = cardData.name;
    }

    [Space(50)]
    public UnityEvent OnCardPlay;
    public UnityEvent OnCardRemove;
    public UnityEvent OnCardDraw;
    public UnityEvent OnCardSell;

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (cardData == null) return;
        if (mesh_CardImage)
            mesh_CardImage.material = cardData.cardImage;
        if (mesh_BackgroundImage)
            mesh_BackgroundImage.material = cardData.cardBackground;
        if (cardName)
            cardName.text = cardData.name;
    }
#endif

    private void OnMouseEnter()
    {
        if (pickedUp) return;
        if (currentTween != null)
            currentTween.Kill();
        currentTween = transform.DOMoveY(0.5f, 0.1f);
    }

    private void OnMouseExit()
    {
        if (pickedUp) return;
        if (currentTween != null)
            currentTween.Kill();
        currentTween = transform.DOMoveY(0f, 0.1f);
    }

    private void OnMouseDown()
    {
        Debug.Log("Grabbed <" + cardData.name + ">");
        pickedUp = true;
        offset = transform.position - GameManager.Instance.MousePosition;
        offset.y = 0f;
    }

    private void OnMouseDrag()
    {
        transform.position = GameManager.Instance.MousePosition + offset;
    }

    private void OnMouseUp()
    {
        Debug.Log("Dropped <" + cardData.name + ">");
        pickedUp = false;
        if (currentTween != null)
            currentTween.Kill();
        currentTween = transform.DOMoveY(0f, 0.1f);
    }
}

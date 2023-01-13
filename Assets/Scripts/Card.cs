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

    public bool PickedUp => pickedUp;

    private Tween currentTween;
    private bool pickedUp;
    private Vector3 offset;
    private Card triggerHit;
    private Vector3 behindOffset;

    private Card cardBehind;
    private Card cardInFront;

    private void Start()
    {
        if (cardData == null) return;
        if (mesh_CardImage)
            mesh_CardImage.material = cardData.cardImage;
        if (mesh_BackgroundImage)
            mesh_BackgroundImage.material = cardData.cardBackground;
        if (cardName)
            cardName.text = cardData.name;

        behindOffset = new Vector3(0f, -0.01f, -1.4f);
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
        currentTween = transform.DOMoveY(0.011f, 0.1f);
    }

    private void OnMouseDown()
    {
        Debug.Log("Grabbed <" + cardData.name + ">");
        transform.parent = null;
        pickedUp = true;
        offset = transform.position - GameManager.Instance.MousePosition;
        offset.y = 0f;
    }

    private void OnMouseDrag()
    {
        transform.position = GameManager.Instance.MousePosition + offset;

        if (Input.GetMouseButtonDown(1))
        {
            if (cardBehind != null)
            {
                cardBehind.RemoveFromBehind();
            }
        }
    }

    private void OnMouseUp()
    {
        Debug.Log("Dropped <" + cardData.name + ">");
        pickedUp = false;
        if (currentTween != null)
            currentTween.Kill();
        currentTween = transform.DOMoveY(0f, 0.1f);

        //Check triggerHit and see if we need to attach ourselves to it.
        if (triggerHit == null) return;

        triggerHit.PutCardBehind(this);
        cardBehind = triggerHit;
        triggerHit = null;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!pickedUp) return;

        Card card = other.gameObject.GetComponent<Card>();
        if (card != null)
        {
            triggerHit = card;
            Debug.Log("Hit: " + triggerHit.cardData.name);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!pickedUp) return;

        Card card = other.gameObject.GetComponent<Card>();
        if (card != null && triggerHit != null && triggerHit == card)
        {
            Debug.Log("Removed TriggerHit Reference");
            triggerHit = null;
        }
    }

    public void EnableCollider(bool enable = true)
    {
        GetComponent<BoxCollider>().enabled = enable;
    }

    public void RemoveFromBehind()
    {
        transform.parent = null;
        cardInFront = null;
        transform.DOMoveY(0f, 0.1f);
        EnableCollider();
    }

    public void PutCardBehind(Card inFront)
    {
        cardInFront = inFront;
        transform.parent = inFront.transform;
        transform.DOLocalMove(behindOffset, 0.1f);
        EnableCollider(false);
    }
}

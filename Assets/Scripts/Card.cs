using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;
using TMPro;
using QFSW.QC;

public class Card : MonoBehaviour
{
    public CardData cardData;
    public MeshRenderer mesh_CardImage;
    public MeshRenderer mesh_BackgroundImage;
    public TMP_Text cardName;
    public TMP_Text cardSellCost;
    public GameObject audioSourcePrefab;
    public GameObject vfxPrefab;
    public BoxCollider mainCollider;
    public BoxCollider behindCollider;
    public Card cardBehind;
    public Card cardInFront;

    public bool PickedUp => pickedUp;

    private Tween currentTween;
    private bool pickedUp;
    private Vector3 offset;
    private Card triggerHit;
    private Vector3 behindOffset;
    private List<Card> stackedCards = new();
    private bool sellTriggerHit;

    [Command]
    private void PerformAction(ActionType action)
    {
        PlaySound(action);
        PlayVFX(action);
    }

    private void PlaySound(ActionType action)
    {
        foreach (CardAction ca in cardData.actions)
        {
            if (ca.action != action) continue;
            if (ca.sound.Length <= 0)
                Debug.LogWarning($"WARNING: {cardData.name} has no sounds in it's list. Is this correct?");
            foreach (AudioClip clip in ca.sound)
            {
                AudioSource source = Instantiate(audioSourcePrefab, transform).GetComponent<AudioSource>();
                source.clip = clip;
                source.Play();
                source.gameObject.name = action.ToString();
            }
        }
    }
    private void PlayVFX(ActionType action)
    {
        foreach (CardAction ca in cardData.actions)
        {
            if (ca.action != action) continue;
            if (ca.vfx.Length <= 0)
                Debug.LogWarning($"WARNING: {cardData.name} has no vfx's in it's list. Is this correct?");
            foreach (GameObject vfx in ca.vfx)
            {
                GameObject go = Instantiate(vfx, transform);
                go.gameObject.name = action.ToString();
            }
        }
    }

    private void Start()
    {
        if (cardData == null) return;
        if (mesh_CardImage)
            mesh_CardImage.material = cardData.cardImage;
        if (mesh_BackgroundImage)
            mesh_BackgroundImage.material = cardData.cardBackground;
        if (cardName)
            cardName.text = cardData.name;
        if (cardSellCost)
            cardSellCost.text = cardData.sellCost.ToString();

        behindOffset = new Vector3(0f, -0.002f, -0.035f);

        PerformAction(ActionType.CREATED);
    }

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
        currentTween = transform.DOMoveY(0.01f, 0.1f);
    }

    private void OnMouseExit()
    {
        if (pickedUp) return;
        if (currentTween != null)
            currentTween.Kill();
        currentTween = transform.DOMoveY(0.0f, 0.1f);
    }

    private void OnMouseDown()
    {
        EnableMainCollider();
        transform.parent = null;
        pickedUp = true;
        offset = transform.position - GameManager.Instance.MousePosition;
        offset.y = 0f;
        PerformAction(ActionType.HOLDING);
    }

    private void OnMouseDrag()
    {
        transform.position = GameManager.Instance.MousePosition + offset;

        if (Input.GetMouseButtonDown(1))
        {
            if (cardBehind != null)
            {
                cardBehind.RemoveLastCardInStack();
            }
        }
    }

    private void OnMouseUp()
    {
        RemoveVFX(ActionType.HOLDING);
        PerformAction(ActionType.DROP);

        pickedUp = false;
        if (currentTween != null)
            currentTween.Kill();

        if (sellTriggerHit)
        {
            SellCard();
            return;
        }

        currentTween = transform.DOMoveY(0f, 0.1f);

        //Check triggerHit and see if we need to attach ourselves to it.
        if (triggerHit == null) return;

        triggerHit.PutCardBehind(this);
        triggerHit = null;

        UpdateStackList();
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

        if (other.gameObject.layer == LayerMask.NameToLayer("SellArea"))
        {
            sellTriggerHit = true;
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

        if (other.gameObject.layer == LayerMask.NameToLayer("SellArea"))
        {
            sellTriggerHit = false;
        }
    }

    public void EnableMainCollider(bool enable = true)
    {
        mainCollider.enabled = enable;
        //behindCollider.enabled = !enable;
    }

    public void RemoveLastCardInStack()
    {
        Card removeLast = this;
        while (removeLast.cardBehind != null)
        {
            removeLast = removeLast.cardBehind;
        }

        removeLast.transform.parent = null;
        removeLast.cardInFront.cardBehind = null;
        removeLast.cardInFront = null;
        if (!pickedUp)
            removeLast.transform.DOMoveY(0f, 0.1f);
        removeLast.EnableMainCollider();
    }

    public void PutCardBehind(Card inFront = null)
    {
        Card toPutBehind = inFront;
        if (toPutBehind != null)
        {
            while (toPutBehind.cardBehind != null)
            {
                toPutBehind = toPutBehind.cardBehind;
            }
        }

        toPutBehind.cardBehind = this;
        cardInFront = toPutBehind;
        transform.parent = toPutBehind.transform;
        transform.DOLocalMove(behindOffset, 0.1f);
        EnableMainCollider(false);
        PerformAction(ActionType.STACK);
    }

    private void UpdateStackList()
    {
        if (cardInFront != null) return;

        stackedCards.Clear();
        stackedCards.Add(this);
        foreach (Card card in GetComponentsInChildren<Card>())
        {
            stackedCards.Add(card);
        }
    }

    private void RemoveVFX(ActionType action)
    {
        foreach (var vfx in GetComponentsInChildren<ParticleSystem>())
        {
            if (vfx.transform.parent.gameObject.name == action.ToString())
                Destroy(vfx.gameObject);
        }
    }

    private void SellCard()
    {
        if (cardBehind)
            cardBehind.SellCard();

        GameManager.Instance.AddCurreny(cardData.sellCost);
        Destroy(gameObject);
    }
}

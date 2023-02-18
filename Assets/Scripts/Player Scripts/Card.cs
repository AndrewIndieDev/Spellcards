using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using QFSW.QC;

public class Card : MonoBehaviour
{
    public Transform timeToCombineTransform;
    public RecipeTimer timeToCombineParent;
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
    public Vector3 behindOffset;
    public MeshRenderer flashingOutline;
    public Transform effectParent;

    private Tween currentTween;
    private bool pickedUp;
    private Vector3 offset;
    private Card triggerHit;
    public List<Card> stackedCards = new();
    private bool sellTriggerHit;
    private Material sellTriggerMaterial;
    private int timeToCombine = 5;
    private bool coroutineRunning;
    private bool cardStackChanged;
    
    private bool spellTriggerHit;
    private SpellArea spellArea;

    [Command]
    public void PerformAction(ActionType action)
    {
        PlaySound(action);
        PlayVFX(action);
    }

    private void PlaySound(ActionType action)
    {
        foreach (CardAction ca in cardData.actions)
        {
            if (ca.action != action) continue;
            if (ca.sfx.Length <= 0)
                Debug.LogWarning($"WARNING: {cardData.name} has no sounds in it's list. Is this correct?");
            foreach (GameObject sfx in ca.sfx)
            {
                AudioSource source = Instantiate(audioSourcePrefab, effectParent).GetComponent<AudioSource>();
                source.Play();
                sfx.name = action.ToString();

                if (sfx.GetComponent<PlayEffectOnChildren>() != null)
                    if (cardBehind)
                        cardBehind.PerformAction(action);
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
                GameObject go = Instantiate(vfx, effectParent);
                go.gameObject.name = action.ToString();

                if (go.GetComponent<PlayEffectOnChildren>() != null)
                    if (cardBehind)
                        cardBehind.PerformAction(action);
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

        GameManager.Instance.OnGameEnd += GameEnd;
        GameManager.Instance.OnCardPickup += OnCardPickup;
        GameManager.Instance.OnCardDrop += OnCardDrop;

        PerformAction(ActionType.CARD_CREATE);
    }

    private void GameEnd()
    {
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        GameManager.Instance.OnGameEnd -= GameEnd;
        GameManager.Instance.OnCardPickup -= OnCardPickup;
        GameManager.Instance.OnCardDrop -= OnCardDrop;
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
        if (pickedUp || cardInFront) return;
        if (currentTween != null)
            currentTween.Kill();
        currentTween = transform.DOLocalMoveY(0.01f, 0.1f);
    }

    private void OnMouseExit()
    {
        if (pickedUp || cardInFront) return;
        if (currentTween != null)
            currentTween.Kill();
        currentTween = transform.DOLocalMoveY(0.0f, 0.1f);
    }

    private void OnMouseDown()
    {
        EnableMainCollider();
        transform.parent = null;
        pickedUp = true;
        offset = transform.position - GameManager.Instance.MousePosition;
        offset.y = 0.02f;
        PerformAction(ActionType.CARD_HOLDING);
        UpdateStackList();
        GameManager.Instance.CardPickup(this);
    }

    private void OnMouseDrag()
    {
        transform.position = GameManager.Instance.MousePosition + offset;

        if (Input.GetMouseButtonDown(1))
        {
            if (cardBehind != null)
            {
                RemoveLastCardInStack();
                UpdateStackList();
            }
        }
    }

    private void OnMouseUp()
    {
        GameManager.Instance.CardDrop();
        RemoveVFX(ActionType.CARD_HOLDING);
        PerformAction(ActionType.CARD_DROP);

        pickedUp = false;

        if (sellTriggerHit)
        {
            SellCard();
            return;
        }

        if (spellTriggerHit)
        {
            ActivateSpell();
            return;
        }

        //Check if we are still on the table or not
        Physics.Raycast(transform.position, Vector3.up, out RaycastHit hit, 1f);
        Debug.Log(hit.collider);
        if (hit.collider != null)
        {
            if (currentTween != null)
                currentTween.Kill();
            currentTween = transform.DOMove(Vector3.zero, 0.1f);
            return;
        }


        //Check triggerHit and see if we need to attach ourselves to it.
        if (triggerHit == null)
        {
                if (currentTween != null)
                    currentTween.Kill();
                currentTween = transform.DOLocalMoveY(0f, 0.1f);
                return;
        }
        if (triggerHit.cardData.isSpell || cardData.isSpell) return;

        triggerHit.PutCardBehind(this);
        triggerHit = null;

        UpdateStackList();

        if (CheckForRecipeSuitability())
        {
            StartCoroutine(RecipeTimer());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!pickedUp || spellTriggerHit) return;

        Card card = other.gameObject.GetComponent<Card>();
        if (card != null && card.stackedCards.Count < 3 &&stackedCards.Count < 3)
        {
            triggerHit = card;
        }

        if (other.gameObject.layer == LayerMask.NameToLayer("SellArea"))
        {
            sellTriggerHit = true;
            sellTriggerMaterial = other.gameObject.GetComponent<MeshRenderer>().material;
            sellTriggerMaterial.SetFloat("_FlashingOpacity", 1f);
        }

        if (!cardData.isSpell || cardData.cantPutInTomb) return;

        if (other.gameObject.layer == LayerMask.NameToLayer("SpellArea"))
        {
            spellTriggerHit = true;
            spellArea = other.gameObject.GetComponent<SpellArea>();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!pickedUp || spellTriggerHit) return;

        Card card = other.gameObject.GetComponent<Card>();
        if (card != null && triggerHit != null && triggerHit == card)
        {
            triggerHit = null;
        }

        if (other.gameObject.layer == LayerMask.NameToLayer("SellArea"))
        {
            sellTriggerHit = false;
            sellTriggerMaterial = other.gameObject.GetComponent<MeshRenderer>().material;
            sellTriggerMaterial.SetFloat("_FlashingOpacity", 0f);
        }

        if (!cardData.isSpell) return;

        if (other.gameObject.layer == LayerMask.NameToLayer("SpellArea"))
        {
            spellTriggerHit = false;
            spellArea = null;
        }
    }

    private void ActivateSpell()
    {
        spellArea.AddCard(this);
        DestroyCardStack();
    }

    public void EnableMainCollider(bool enable = true)
    {
        mainCollider.enabled = enable;
        //behindCollider.enabled = !enable;
    }

    public void RemoveLastCardInStack()
    {
        Card cardToRemove = this;
        while (cardToRemove.cardBehind != null)
        {
            cardToRemove = cardToRemove.cardBehind;
        }

        cardToRemove.RemoveCardFromThisCardsStack(this);
    }

    public void RemoveCardFromThisCardsStack(Card stackLeader)
    {
        if (stackLeader.stackedCards.Count > 0)
            stackLeader.stackedCards.RemoveAt(stackLeader.stackedCards.Count - 1);
        stackLeader.cardStackChanged = true;
        transform.parent = null;
        cardInFront.cardBehind = null;
        cardInFront = null;
        if (!pickedUp)
            transform.DOLocalMoveY(0f, 0.1f);
        EnableMainCollider();
        PerformAction(ActionType.CARD_DROP);
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

        EnableMainCollider(false);
        toPutBehind.cardBehind = this;
        cardInFront = toPutBehind;
        transform.parent = toPutBehind.transform;
        if (currentTween != null)
            currentTween.Kill();
        currentTween = transform.DOLocalMove(behindOffset, 0.1f);
        UpdateStackList();
        PerformAction(ActionType.CARD_STACK);
    }

    private IEnumerator RecipeTimer()
    {
        coroutineRunning = true;
        ResetRecipeCreation(true);
        //timeToCombineParent.gameObject.SetActive(true);
        Tween tween = timeToCombineTransform.DOScaleX(1f, timeToCombine);
        while (timeToCombineTransform.localScale.x < 1f || pickedUp)
        {
            if (cardInFront || stackedCards.Count == 0)
            {
                tween?.Kill();
                ResetRecipeCreation(false);
                break;
            }
            if (cardStackChanged)
            {
                //timeToCombineTransform.transform.localScale = new Vector3(0f, 1f, 1f);
                cardStackChanged = false;
                ResetRecipeCreation(true);
            }
            yield return null;
        }

        if (!cardStackChanged && !cardInFront && stackedCards.Count != 0)
        {
            Recipe[] recipes = Resources.LoadAll<Recipe>("Recipes");
            bool spawned = false;
            int cardsInRecipe = 0;
            int cardsInStack = 0;
            foreach (Recipe recipe in recipes)
            {
                bool inRecipe = true;
                cardsInRecipe = recipe.recipeCards.Count;
                cardsInStack = stackedCards.Count;
                if (cardsInRecipe != cardsInStack)
                    continue;
                foreach (Card card in stackedCards)
                {
                    if (!recipe.recipeCards.Contains(card.cardData))
                    {
                        inRecipe = false;
                        break;
                    }
                }
                if (inRecipe)
                {
                    GameManager.Instance.SpawnCard(recipe.result, transform.position);
                    spawned = true;
                    break;
                }
            }
            if (!spawned)
                GameManager.Instance.SpawnCard(GameManager.Instance.failedCreationRef, transform.position);
            DestroyCardStack();
        }
        coroutineRunning = false;
    }

    private void UpdateStackList()
    {
        stackedCards.Clear();

        if (cardInFront != null) return;
        if (cardBehind == null) return;

        foreach (Card card in GetComponentsInChildren<Card>())
        {
            stackedCards.Add(card);
        }
        cardStackChanged = true;
    }

    private void RemoveVFX(ActionType action)
    {
        for (int i = effectParent.childCount - 1; i >= 0; i--)
        {
            if (effectParent.GetChild(i).gameObject.name == action.ToString())
                Destroy(effectParent.GetChild(i).gameObject);
        }
    }

    private void SellCard()
    {
        if (cardBehind)
            cardBehind.SellCard();

        GameManager.Instance.AddCurrency(cardData.sellCost);
        sellTriggerMaterial?.SetFloat("_FlashingOpacity", 0f);
        Destroy(gameObject);
    }

    public void DestroyCardStack()
    {
        if (cardBehind)
            cardBehind.DestroyCardStack();

        Destroy(gameObject);
    }

    private bool CheckForRecipeSuitability()
    {
        return cardBehind != null && !coroutineRunning;
    }

    private void ResetRecipeCreation(bool keepEnabled)
    {
        timeToCombineTransform.localScale = new Vector3(0f, 1f, 1f);
        timeToCombineParent.gameObject.SetActive(keepEnabled);
    }

    private void OnCardPickup(Card card)
    {
        if (card == null || card == this) return;
        if (card.cardData.potentialRecipeCard.Contains(cardData))
            flashingOutline.material.SetFloat("_Opacity", 1f);
    }

    private void OnCardDrop()
    {
        flashingOutline.material.SetFloat("_Opacity", 0f);
    }
}

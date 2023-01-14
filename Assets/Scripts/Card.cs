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

    public bool PickedUp => pickedUp;

    private Tween currentTween;
    private bool pickedUp;
    private Vector3 offset;
    private Card triggerHit;
    private Vector3 behindOffset;
    public List<Card> stackedCards = new();
    private bool sellTriggerHit;
    private Material sellTriggerMaterial;
    private int timeToCombine = 5;
    private bool coroutineRunning;
    private bool cardStackChanged;
    
    private bool spellTriggerHit;
    private SpellArea spellArea;

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
        currentTween = transform.DOLocalMoveY(0.01f, 0.1f);
    }

    private void OnMouseExit()
    {
        if (pickedUp) return;
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
        offset.y = 0f;
        PerformAction(ActionType.HOLDING);
        UpdateStackList();
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
        RemoveVFX(ActionType.HOLDING);
        PerformAction(ActionType.DROP);

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

        if (currentTween != null)
            currentTween.Kill();
        currentTween = transform.DOLocalMoveY(0f, 0.1f);

        //Check triggerHit and see if we need to attach ourselves to it.
        if (triggerHit == null) return;
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

        if (!cardData.isSpell) return;

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
        transform.position = spellArea.gameObject.transform.parent.position;
        transform.rotation = spellArea.gameObject.transform.parent.rotation;
        spellArea.SetCard(this);
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
            removeLast.transform.DOLocalMoveY(0f, 0.1f);
        removeLast.EnableMainCollider();
        stackedCards.RemoveAt(stackedCards.Count - 1);
        cardStackChanged = true;
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
        UpdateStackList();
        PerformAction(ActionType.STACK);
    }

    private IEnumerator RecipeTimer()
    {
        coroutineRunning = true;
        ResetRecipeCreation();
        timeToCombineParent.gameObject.SetActive(true);
        Tween tween = timeToCombineTransform.DOScaleX(1f, timeToCombine);
        while (timeToCombineTransform.localScale.x < 1f || pickedUp)
        {
            if (cardInFront || stackedCards.Count == 0)
            {
                tween?.Kill();
                ResetRecipeCreation();
                break;
            }
            if (cardStackChanged)
            {
                timeToCombineTransform.transform.localScale = new Vector3(0f, 1f, 1f);
                cardStackChanged = false;
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

    private void ResetRecipeCreation()
    {
        timeToCombineTransform.localScale = new Vector3(0f, 1f, 1f);
        timeToCombineParent.gameObject.SetActive(false);
    }
}

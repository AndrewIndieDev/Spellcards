using QFSW.QC;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellArea : MonoBehaviour
{
    public float spellChargeTime;
    public bool IsInUse => cardData != null;
    public Transform chargeTransform;
    public FakeSpellCard visual;
    public Transform spellFireTransform;

    [Header("DEBUG STUFF")]
    [SerializeField] private CardData testSpell;

    private CardData cardData;
    private List<CardData> queue = new();

    public void AddCard(Card data)
    {
        queue.Add(data.cardData);
        for (int i = 1; i < data.stackedCards.Count; i++)
        {
            queue.Add(data.stackedCards[i].cardData);
        }
        if (!IsInUse)
            StartCoroutine(ChargeSpell());
    }

    private IEnumerator ChargeSpell()
    {
        while (queue.Count > 0 && GameManager.Instance.playing)
        {
            cardData = queue[0];
            visual.gameObject.SetActive(true);
            visual.Init(cardData);
            float time = spellChargeTime;
            while (time > 0)
            {
                time -= Time.deltaTime;
                chargeTransform.localScale = new Vector3((spellChargeTime - time) / spellChargeTime, 1f, 1f);
                yield return null;
            }
            if (GameManager.Instance.playing)
            {
                chargeTransform.localScale = new Vector3(0f, 1f, 1f);
                LaunchSpell();
                cardData = null;
                queue.RemoveAt(0);
                visual.gameObject.SetActive(false);
            }
        }
    }

    private void LaunchSpell()
    {
        Spell spell = Instantiate(cardData.spellPrefab, spellFireTransform.position, spellFireTransform.rotation, spellFireTransform).GetComponent<Spell>();
        spell.SetData(cardData);
    }

    [Command]
    private void DebugTestSpell(int amount)
    {
        StartCoroutine(DebugTestSpellEnumerator(amount));
    }

    private IEnumerator DebugTestSpellEnumerator(int amount)
    {
        while (amount > 0)
        {
            if (!GameManager.Instance.playing)
                break;
            cardData = testSpell;
            LaunchSpell();
            amount--;
            yield return new WaitForSeconds(1f);
        }
    }
}

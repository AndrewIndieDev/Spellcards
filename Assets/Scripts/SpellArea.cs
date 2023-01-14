using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellArea : MonoBehaviour
{
    public float spellChargeTime;
    public bool IsInUse => cardData != null;
    public Transform chargeTransform;

    private Card cardData;

    public void SetCard(Card data)
    {
        cardData = data;
        StartCoroutine(ChargeSpell());
    }

    private IEnumerator ChargeSpell()
    {
        float time = spellChargeTime;
        while(time > 0)
        {
            time -= Time.deltaTime;
            chargeTransform.localScale = new Vector3((spellChargeTime - time) / spellChargeTime, 1f, 1f);
            yield return null;
        }
        chargeTransform.localScale = new Vector3(0f, 1f, 1f);
        cardData.DestroyCardStack();
        cardData = null;
    }
}

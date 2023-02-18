using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FakeSpellCard : MonoBehaviour
{
    public TMP_Text cost;
    public new TMP_Text name;
    public MeshRenderer item;
    public MeshRenderer forground;

    public void Init(CardData data)
    {
        cost.text = data.sellCost.ToString();
        name.text = data.name;
        item.material = data.cardImage;
        forground.material = data.cardBackground;
    }
}

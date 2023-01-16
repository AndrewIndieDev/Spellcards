using UnityEngine;
using DG.Tweening;
using TMPro;

public class DamagePopup : MonoBehaviour
{
    public TMP_Text damage;

    private void Start()
    {
        transform.position = new Vector3(Random.Range(-.8f, .8f), 1.8f, 1.35f);
    }

    public void Init(int amount)
    {
        damage.text = amount.ToString();
        if (amount > 100000)
            damage.color = Color.red;
        Tween t = transform.DOMoveY(1.6f, 1f);
        t.onComplete += () =>
        {
            Destroy(gameObject);
        };
    }
}

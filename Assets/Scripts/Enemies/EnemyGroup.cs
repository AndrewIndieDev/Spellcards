using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EnemyGroup : MonoBehaviour
{
    public Transform spellHitTransform;
    public DamagePopup damagePopupPrefab;

    private Tween currentTween;
    [SerializeField] private int health = 300000;

    private void Start()
    {
        GameManager.Instance.OnGameEnd += GameEnd;
        currentTween = transform.DOMove(WaveSpawner.Instance.GetEndPosition, 120f);
        currentTween.onComplete += () =>
        {
            StartCoroutine(DealDamage());
        };
    }

    private void OnDestroy()
    {
        GameManager.Instance.OnGameEnd -= GameEnd;
        GameManager.Instance.OnEnemyGroupDies(this);
    }

    public void GameEnd()
    {
        if (currentTween != null)
            currentTween.Kill();
        Destroy(gameObject);
    }

    public void TakeDamage(int damage)
    {
        health = (health - damage > 0) ? health - damage : 0;
        DamagePopup dp = Instantiate(damagePopupPrefab);
        dp.Init(damage);
        if (health <= 0)
            Destroy(gameObject);
    }

    private IEnumerator DealDamage()
    {
        yield return null;
        // Deal damage
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BigBad : MonoBehaviour
{
    public static BigBad Instance;
    private void Awake()
    {
        Instance = this;
    }

    public Transform endPos;
    public Transform spellHitTransform;
    public DamagePopup damagePopupPrefab;

    private Vector3 startPos;
    private Tween currentTween;
    private int bigBadHp = 1000000;

    private void Start()
    {
        startPos = transform.position;
        GameManager.Instance.OnGameStart += GameStart;
        GameManager.Instance.OnGameEnd += GameEnd;
    }

    public void GameStart()
    {
        transform.position = startPos;
        currentTween = transform.DOMove(endPos.position, 120f);
        currentTween.onComplete += () =>
        {
            GameManager.Instance.GameEnd();
        };
    }

    public void GameEnd()
    {
        if (currentTween != null)
            currentTween.Kill();
        bigBadHp = 1000000;
    }

    public void TakeDamage(int damage)
    {
        bigBadHp = (bigBadHp - damage > 0) ? bigBadHp - damage : 0;
        BigBadHealth.Instance.UpdateHealth(bigBadHp);
        DamagePopup dp = Instantiate(damagePopupPrefab);
        dp.Init(damage);
        if (bigBadHp <= 0)
            GameManager.Instance.GameEnd();
    }
}

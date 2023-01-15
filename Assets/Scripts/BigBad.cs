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
    
    private Vector3 startPos;
    private Tween currentTween;

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
    }

    public void GameEnd()
    {
        if (currentTween != null)
            currentTween.Kill();
    }
}

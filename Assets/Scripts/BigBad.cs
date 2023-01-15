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
    
    private Vector3 startPos;
    private Tween currentTween;

    private void Start()
    {
        startPos = transform.position;
    }

    public void GameStart()
    {
        currentTween = transform.DOMove(endPos.position, 120f);
    }
}

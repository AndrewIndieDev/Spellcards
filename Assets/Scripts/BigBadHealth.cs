using UnityEngine;
using DG.Tweening;

public class BigBadHealth : MonoBehaviour
{
    public static BigBadHealth Instance;
    private void Awake()
    {
        Instance = this;
    }

    public Transform healthTransform;

    private Tween currentTween;

    private void Start()
    {
        GameManager.Instance.OnGameStart += GameStart;
    }

    public void UpdateHealth(int totalHp)
    {
        if (currentTween != null)
            currentTween.Kill();
        currentTween = healthTransform.DOScaleX((float)totalHp / 1000000f, 0.2f);
    }

    private void GameStart()
    {
        healthTransform.localScale = Vector3.one;
    }
}

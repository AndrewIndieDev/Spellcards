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

    public void UpdateHealth(int totalHp)
    {
        if (currentTween != null)
            currentTween.Kill();
        currentTween = healthTransform.DOScaleX((float)totalHp / 1000000f, 0.2f);
    }
}

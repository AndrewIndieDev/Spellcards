using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public class PhysicalButton : MonoBehaviour
{
    public MeshRenderer mr;
    public UnityEvent action;

    private Tween currentTween;

    private void OnMouseUp()
    {
        action.Invoke();
    }

    private void OnMouseEnter()
    {
        if (currentTween != null)
            currentTween.Kill();
        currentTween = mr.material.DOColor(new Color(1f, 1f, 1f, 0.1f), 0.1f);
    }

    private void OnMouseExit()
    {
        if (currentTween != null)
            currentTween.Kill();
        currentTween = mr.material.DOColor(new Color(1f, 1f, 1f, 0f), 0.1f);
    }
}
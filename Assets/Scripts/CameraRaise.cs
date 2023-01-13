using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CameraRaise : MonoBehaviour
{
    public Camera cam;
    [Range(0,100)]
    public int screenPercentageToLookUp = 10;

    private float screenLookUp;
    private Tween currentTween;
    private bool lookup;

    private void Update()
    {
        screenLookUp = Screen.height - (Screen.height * ((float)screenPercentageToLookUp / 100));
        if (Input.mousePosition.y > screenLookUp && !lookup)
        {
            if (currentTween != null)
                currentTween.Kill();
            currentTween = cam.transform.DORotate(new Vector3(20f, 0f, 0f), 0.5f);
            currentTween.OnComplete(() => { currentTween = null; });
            lookup = true;
        }
        else if (Input.mousePosition.y < screenLookUp && lookup)
        {
            if (currentTween != null)
                currentTween.Kill();
            currentTween = cam.transform.DORotate(new Vector3(60f, 0f, 0f), 0.5f);
            currentTween.OnComplete(() => { currentTween = null; });
            lookup = false;
        }
    }
}

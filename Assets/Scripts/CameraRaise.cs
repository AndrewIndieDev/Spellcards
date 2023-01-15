using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CameraRaise : MonoBehaviour
{
    public static CameraRaise Instance;
    private void Awake()
    {
        Instance = this;
    }

    public Camera cam;
    [Range(0,100)]
    public int screenPercentageToLookUp = 10;

    private float screenLookUp;
    private Tween currentTween;
    private bool lookup;

    private void Start()
    {
        GameManager.Instance.OnGameStart += GameStart;
        GameManager.Instance.OnGameEnd += GameEnd;
    }

    private void Update()
    {
        if (!GameManager.Instance.playing) return;

        screenLookUp = Screen.height - (Screen.height * ((float)screenPercentageToLookUp / 100));

        if (Input.mousePosition.y > screenLookUp && !lookup)
        {
            SetCameraXRotation(20f);
            lookup = true;
            screenPercentageToLookUp = 90;
        }
        else if (Input.mousePosition.y < screenLookUp && lookup)
        {
            SetCameraXRotation(60f);
            lookup = false;
            screenPercentageToLookUp = 10;
        }
    }

    private void SetCameraXRotation(float rot)
    {
        if (currentTween != null)
            currentTween.Kill();
        currentTween = cam.transform.DORotate(new Vector3(rot, 0f, 0f), 0.5f);
        currentTween.OnComplete(() => { currentTween = null; });
    }

    public void GameStart()
    {
        SetCameraXRotation(60f);
    }

    public void GameEnd()
    {
        SetCameraXRotation(-90f);
    }
}

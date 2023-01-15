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
    private Tween rotTween;
    private Tween posTween;
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
            SetCameraRotation(new Vector3(15f, 0f, 0f));
            SetCameraPosition(new Vector3(0f, 1.95f, -1.57f));
            lookup = true;
            screenPercentageToLookUp = 90;
        }
        else if (Input.mousePosition.y < screenLookUp && lookup)
        {
            SetCameraRotation(new Vector3(60f, 0f, 0f));
            SetCameraPosition(new Vector3(0f, 1.5f, -.35f));
            lookup = false;
            screenPercentageToLookUp = 10;
        }
    }

    private void SetCameraRotation(Vector3 rot)
    {
        if (rotTween != null)
            rotTween.Kill();
        rotTween = cam.transform.DORotate(rot, 0.5f);
        rotTween.OnComplete(() => { rotTween = null; });
    }

    private void SetCameraPosition(Vector3 pos)
    {
        if (posTween != null)
            posTween.Kill();
        posTween = cam.transform.DOMove(pos, 0.5f);
        posTween.OnComplete(() => { posTween = null; });
    }

    public void GameStart()
    {
        SetCameraRotation(new Vector3(60f, 0f, 0f));
        SetCameraPosition(new Vector3(0f, 1.5f, -.35f));
    }

    public void GameEnd()
    {
        SetCameraRotation(new Vector3(-90f, 0f, 0f));
        SetCameraPosition(new Vector3(0f, -.8f, -1f));
    }
}

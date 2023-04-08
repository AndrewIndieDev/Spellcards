using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CameraTweening : MonoBehaviour
{
    public static CameraTweening Instance;
    private void Awake()
    {
        Instance = this;
    }

    public Camera cam;
    private Tween rotTween;
    private Tween posTween;

    private void Start()
    {
        GameManager.Instance.OnGameStart += GameStart;
        GameManager.Instance.OnGameEnd += GameEnd;
    }

    private void OnDestroy()
    {
        GameManager.Instance.OnGameStart -= GameStart;
        GameManager.Instance.OnGameEnd -= GameEnd;
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
        SetCameraLookAtTable();
    }

    public void GameEnd()
    {
        SetCameraLookAtMenu();
    }

    private void SetCameraLookAtTable()
    {
        SetCameraRotation(new Vector3(75f, 0f, 0f));
        SetCameraPosition(new Vector3(0f, 1.6f, 0f));
    }

    private void SetCameraLookAtMenu()
    {
        SetCameraRotation(new Vector3(-90f, 0f, 0f));
        SetCameraPosition(new Vector3(0f, -.8f, -1f));
    }
}

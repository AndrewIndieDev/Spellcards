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
    public int screenPercentageToLookRight = 95;

    private float screenLookUp;
    private float screenLookRight;
    private Tween rotTween;
    private Tween posTween;
    private bool lookUp;
    private bool lookRight;

    private void Start()
    {
        GameManager.Instance.OnGameStart += GameStart;
        GameManager.Instance.OnGameEnd += GameEnd;
    }

    private void Update()
    {
        if (!GameManager.Instance.playing) return;

        screenLookUp = Screen.height - (Screen.height * ((float)screenPercentageToLookUp / 100));
        screenLookRight = Screen.width - (Screen.width * ((float)screenPercentageToLookRight / 100));

        if (Input.mousePosition.y > screenLookUp && !lookUp)
        {
            ResetAllCameraProperties();
            SetCameraRotation(new Vector3(15f, 0f, 0f));
            SetCameraPosition(new Vector3(0f, 1.95f, -1.57f));
            screenPercentageToLookUp = 90;
            lookUp = true;
        }
        
        else if (Input.mousePosition.y < screenLookUp && lookUp)
        {
            ResetAllCameraProperties();
            SetCameraRotation(new Vector3(60f, 0f, 0f));
            SetCameraPosition(new Vector3(0f, 1.5f, -.35f));
            screenPercentageToLookUp = 10;
            lookUp = false;
        }

        //if (Input.mousePosition.x > screenLookRight && !lookRight)
        //{
        //    ResetAllCameraProperties();
        //    SetCameraRotation(new Vector3(0f, 90f, 0f));
        //    SetCameraPosition(new Vector3(0f, .8f, -1f));
        //    lookRight = true;
        //}
    }

    private void ResetAllCameraProperties()
    {
        lookRight = false;
        lookUp = false;
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
        lookUp = false;
        lookRight = false;
    }
}

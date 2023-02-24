using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Spell : MonoBehaviour
{
    public GameObject vfxExplosionPrefab;
    [Space(20)]
    public AnimationCurve xCurve;
    public AnimationCurve yCurve;

    private CardData data;
    private Vector3 endPosition;
    private float dist;
    private float parentDist;

    private void Start()
    {
        endPosition = WaveSpawner.Instance.GetClosestEnemyGroup.transform.position;
        dist = Vector3.Distance(transform.position, endPosition);
        parentDist = Vector3.Distance(transform.parent.position, endPosition);
        GameManager.Instance.OnGameEnd += GameEnd;
    }

    private void OnDestroy()
    {
        GameManager.Instance.OnGameEnd -= GameEnd;
    }

    private void GameEnd()
    {
        GameManager.Instance.OnGameEnd -= GameEnd;
        Destroy(gameObject);
    }

    public void SetData(CardData data)
    {
        this.data = data;
    }

    private void Update()
    {
        endPosition = WaveSpawner.Instance.GetClosestEnemyGroup.transform.position;
        dist = Vector3.Distance(transform.position, endPosition);
        parentDist = Vector3.Distance(transform.parent.position, endPosition);
        float x = 0f;
        float y = 0f;

        transform.position = new Vector3(x, y, transform.position.z + Time.deltaTime * 200f);

        if (transform.position.z > endPosition.z)
        {
            //Create explosion
            WaveSpawner.Instance.GetClosestEnemyGroup.TakeDamage(data.spellDamage + Random.Range(-20000, 20001));
            if (vfxExplosionPrefab != null)
            {
                Transform t = Instantiate(vfxExplosionPrefab, transform.position, Quaternion.identity).transform;
                t.DORotate(new Vector3(-90f, 0f, 0f), 0.01f);
            }
            Destroy(gameObject);
        }
    }
}

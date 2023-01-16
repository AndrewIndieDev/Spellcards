using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spell : MonoBehaviour
{
    public GameObject vfxExplosionPrefab;
    [Space(20)]
    public AnimationCurve xCurve;
    public AnimationCurve yCurve;

    private CardData data;
    private Vector3 endPosition;
    private float dist;

    private void Start()
    {
        endPosition = BigBad.Instance.spellHitTransform.position;
        dist = Vector3.Distance(transform.position, endPosition);

        GameManager.Instance.OnGameEnd += GameEnd;
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
        endPosition = BigBad.Instance.spellHitTransform.position;
        float x = xCurve.Evaluate((dist - Vector3.Distance(transform.position, endPosition)) / dist);
        float y = yCurve.Evaluate((dist - Vector3.Distance(transform.position, endPosition)) / dist);

        transform.position = new Vector3(x, y, transform.position.z + Time.deltaTime * 200f);

        if (transform.position.z > endPosition.z)
        {
            //Create explosion
            GameManager.Instance.OnGameEnd -= GameEnd;
            BigBad.Instance.TakeDamage(data.spellDamage + Random.Range(-20000, 20001));
            if (vfxExplosionPrefab != null)
                Instantiate(vfxExplosionPrefab, transform.position, Quaternion.identity, null);
            Destroy(gameObject);
        }
    }
}

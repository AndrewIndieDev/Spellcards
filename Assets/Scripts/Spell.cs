using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Spell : MonoBehaviour
{
    public AnimationCurve xCurve;
    public AnimationCurve yCurve;

    private Vector3 endPosition;
    private float dist;

    private void Start()
    {
        endPosition = BigBad.Instance.spellHitTransform.position;
        dist = Vector3.Distance(transform.position, endPosition);

        Tween t = transform.DOMoveZ(endPosition.z, 5f - (4.5f * ((dist - Vector3.Distance(transform.position, endPosition)) / dist)));
        t.easePeriod = 0f;
    }

    private void Update()
    {
        float x = xCurve.Evaluate(((dist - Vector3.Distance(transform.position, endPosition)) / dist));
        float y = yCurve.Evaluate(((dist - Vector3.Distance(transform.position, endPosition)) / dist));

        transform.position = new Vector3(x, y, transform.position.z);
    }
}

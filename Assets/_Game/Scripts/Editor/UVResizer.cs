using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UVResizer : MonoBehaviour
{

    [SerializeField] private Material currentMaterial;
    [SerializeField] private Collider currentCollider;

    void start()
    {
        currentCollider = transform.GetComponent<Collider>();
    }

    private Vector4 getBounds()
    {
        Vector4 minBounds = currentCollider.bounds.max;
        return minBounds;
    }

    private void setBounds()
    {
        currentMaterial.SetVector("_Bounds", getBounds());
    }
}

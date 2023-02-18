using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterSeconds : MonoBehaviour
{
    public float destroyIn = 5.1f;
    void Start()
    {
        Invoke("D", destroyIn);
    }

    private void D()
    {
        Destroy(gameObject);
    }
}

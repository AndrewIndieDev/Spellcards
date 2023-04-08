using UnityEngine;

public class UVResizer : MonoBehaviour
{
    [SerializeField] private MeshRenderer currentMaterial;
    [SerializeField] private Collider currentCollider;

    private Vector4 getBounds()
    {
        Vector4 minBounds = currentCollider.bounds.extents;
        return minBounds;
    }

    public void setBounds()
    {
        if (currentMaterial == null || currentCollider == null)
        {
            Debug.LogError("MeshRenderer or Collider not set!");
            return;
        }
        currentMaterial.sharedMaterial.SetVector("_Bounds", getBounds());
    }
}

/*
private Vector4 getBounds()
{
    Mesh mesh = GetComponent<MeshFilter>().mesh;
    Bounds bounds = mesh.bounds;
    Vector4 boundsSize = bounds.size;
    return boundsSize;
}

public void setBounds()
{
    if (currentMaterial == null)
    {
        Debug.LogError("MeshRenderer or Collider not set!");
        return;
    }
    currentMaterial.sharedMaterial.SetVector("_Bounds", getBounds());
}
}
*/
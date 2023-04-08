using Sirenix.OdinInspector;
using System;
using UnityEngine;

public class SpellAbility : MonoBehaviour
{
    [Title("Inspector References")]
    [SerializeField] private LineRenderer r_lineRenderer;
    [SerializeField] private MeshRenderer r_startOfLineMeshRenderer;
    [SerializeField] private MeshRenderer r_endOfLineMeshRenderer;

    [Title("Inspector Variables")]
    [SerializeField] private SpellAbilityData r_data;

    [Title("Read Only Variables")]
    [ReadOnly][SerializeField] private Vector3 v_targetPosition;

    #region Unity Methods
    private void Start()
    {
        InitData();
    }
    private void Update()
    {
        v_targetPosition = GameManager.Instance.MousePosition; // Change this to grid position once grid has been added back in.
        UpdateLine();
    }
    #endregion

    #region Public Methods

    #endregion

    #region Private Variables
    /// <summary>
    /// Initializes the data for this spell ability.
    /// </summary>
    private void InitData()
    {
        UpdateLine();
    }
    /// <summary>
    /// Updates the line renderer for this spell ability.
    /// </summary>
    private void UpdateLine()
    {
        r_lineRenderer.enabled = r_data.hasLine;
        r_lineRenderer.startWidth = r_data.line_Thickness;
        r_lineRenderer.endWidth = r_data.line_Thickness;
        r_startOfLineMeshRenderer.material.mainTexture =
            r_data.line_StartSprite != null ? r_data.line_StartSprite.texture : r_startOfLineMeshRenderer.material.mainTexture;
        r_endOfLineMeshRenderer.material.mainTexture =
            r_data.line_EndSprite != null ? r_data.line_EndSprite.texture : r_endOfLineMeshRenderer.material.mainTexture;
    }
    #endregion
}

[Serializable]
public struct SpellAbilityData
{
    [Title("Line Renderer Settings")]
    public bool hasLine;
    public Sprite line_StartSprite;
    public Sprite line_EndSprite;
    public float line_Thickness;
    public Color line_Color;

    [Title("Generic Settings")]
    public float maxDistance;
}
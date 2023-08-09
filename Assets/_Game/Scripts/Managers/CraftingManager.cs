using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

public class CraftingManager : MonoBehaviour
{
    private GridManager Grid => GridManager.Instance;

    private Vector2Int GridPosition => Grid.GetGridCoordsAtWorldPosition(transform.position);

    [Title("Inspector References")]

    #region Unity Methods
    private void Start()
    {
        
    }
    #endregion

    #region Public Methods

    #endregion

    #region Private Methods

    #endregion
}
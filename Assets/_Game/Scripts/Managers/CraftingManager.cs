using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

public class CraftingManager : MonoBehaviour
{
    private GridManager Grid => GridManager.Instance;

    private Vector2Int GridPosition => Grid.GetGridCoordsAtWorldPosition(transform.position);

    [Title("Inspector References")]
    [SerializeField] private List<Vector2Int> slotGridCells;
    [SerializeField] private GameObject p_Slot;

    #region Unity Methods
    private void Start()
    {
        for (int i = 0; i < 3; i++)
        {
            Vector2Int gridCell = GridPosition + new Vector2Int(i - 1, 1);
            slotGridCells.Add(gridCell);
            GameObject go = Instantiate(p_Slot, transform);
            go.transform.position = Grid.GetGridCellCenterPosition(gridCell);
        }
    }
    #endregion

    #region Public Methods

    #endregion

    #region Private Methods

    #endregion
}
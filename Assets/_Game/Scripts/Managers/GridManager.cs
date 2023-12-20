using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public enum EGridCellOccupiedFlags
{
    None,
    Enemy,
    Unselectable,
    Card,


    Count
}

public class GridCell
{
    public EGridCellOccupiedFlags occupiedFlag = EGridCellOccupiedFlags.None;
    public Dictionary<EGridCellOccupiedFlags, IPlaceable> occupiers = new Dictionary<EGridCellOccupiedFlags, IPlaceable>();
    public float weight = 1f;
}

public class GridManager : MonoBehaviour
{
    public static GridManager Instance;
    private void Awake()
    {
        Instance = this;
    }

    public delegate void OnSelectionGridPositionChanged(Vector2Int cellPosition);
    public static event OnSelectionGridPositionChanged onSelectionGridPositionChanged;
    public delegate void OnGridCellChanged(Vector2Int cellPosition);
    public static event OnGridCellChanged onGridCellChanged;

    public static EGridCellOccupiedFlags playerBlocked = EGridCellOccupiedFlags.Unselectable;

    public Vector3 SelectionPositionWorld { get { return selection.transform.position + new Vector3(gridHorizontalSize / 2f, 0f, gridVerticalSize / 2f); } }
    public Vector2Int SelectionPositionGrid { get { return GetGridCoordsAtWorldPosition(selection.transform.position); } }

    // NEED TO MANUALLY CHANGE //
    public int GridWidth { get { return 24; } }
    public int GridHeight { get { return 14; } }
    public int EnemyRows { get { return 2; } }
    /////////////////////////////

    public float gridVerticalSize;
    public float gridHorizontalSize;
    public GameObject selection;
    public LayerMask groundLayer;

    private Dictionary<Vector2Int, GridCell> occupiedGridCells = new Dictionary<Vector2Int, GridCell>();

    private void Start()
    {
        selection = Instantiate(selection);
        selection.transform.localScale = new Vector3(gridHorizontalSize, 0.01f, gridVerticalSize);

        for (int x = 0; x < GridWidth; x++)
        {
            SpawningManager.Instance.SpawnCard(SpawningManager.Instance.spikedWall_debug, new Vector2Int(x, GridHeight - 5));
        }
    }

    //private void Update()
    //{
    //    if (Input.GetMouseButtonDown(1))
    //    {
    //        Vector2Int position = SelectionPositionGrid;
    //        GridCell clicked = GetGridCell(position);
    //        Debug.Log($"{position.x}, {position.y} | {((clicked != null) ? clicked.occupiedFlag : "<NULL>")}");
    //    }
    //}

    public void OnNavigationMouse(InputAction.CallbackContext context)
    {
        Ray ray = Camera.main.ScreenPointToRay(context.ReadValue<Vector2>());
        if (Physics.Raycast(ray, out RaycastHit hit, 99999f, groundLayer))
        {
            Vector2Int gridPosition = GetGridCoordsAtWorldPosition(hit.point);
            gridPosition = ClampPositionToPlayerGrid(gridPosition);
            MoveSelection(gridPosition);
        }
    }

    public Vector2Int GetGridCoordsAtWorldPosition(Vector3 worldPosition)
    {
        Vector3 relativePosition = worldPosition - transform.position;
        Vector3 gridPosition = Vector3.zero;
        gridPosition.x = Mathf.FloorToInt(relativePosition.x / gridHorizontalSize + gridHorizontalSize * 0.01f);
        gridPosition.z = Mathf.FloorToInt(relativePosition.z / gridVerticalSize + gridVerticalSize * 0.01f);
        return new Vector2Int((int)gridPosition.x, (int)gridPosition.z);
    }

    public Vector3 GetGridCellPosition(Vector2Int gridPosition)
    {
        return transform.position + new Vector3(gridPosition.x * gridHorizontalSize, 0.0f, gridPosition.y * gridVerticalSize);
    }

    public Vector3 GetGridCellPosition(Vector3 worldPosition)
    {
        return GetGridCellPosition(GetGridCoordsAtWorldPosition(worldPosition));
    }

    public Vector3 GetGridCellCenterPosition(Vector2Int gridPosition)
    {
        return GetGridCellPosition(gridPosition) + new Vector3(gridHorizontalSize / 2.0f, 0.0f, gridVerticalSize / 2.0f);
    }

    public void ResizeSelection(Vector2Int size)
    {
        Vector3 newSize = new(size.x * gridHorizontalSize, 0, size.y * gridVerticalSize);
        selection.transform.localScale = newSize.IgnoreAxis(EAxis.Y, 0.1f);
    }

    public void ResetSelection()
    {
        selection.transform.localScale = new Vector3(gridHorizontalSize, 0, gridVerticalSize).IgnoreAxis(EAxis.Y, 0.1f);
    }

    public bool GridPositionFree(Vector2Int position, EGridCellOccupiedFlags flagToCheck = EGridCellOccupiedFlags.Card)
    {
        if (occupiedGridCells.TryGetValue(position, out GridCell gridCell))
        {
            return gridCell.occupiedFlag == EGridCellOccupiedFlags.None || gridCell.occupiedFlag != flagToCheck;
        }
        return true;
    }

    public bool GridPositionsFree(List<Vector2Int> positions, EGridCellOccupiedFlags flagsToCheck = EGridCellOccupiedFlags.Card)
    {
        foreach(Vector2Int position in positions)
        {
            if (!GridPositionFree(position, flagsToCheck))
                return false;
        }
        return true;
    }

    public bool GridPositionsFree(Vector2Int[] positions, EGridCellOccupiedFlags flagsToCheck = EGridCellOccupiedFlags.Card)
    {
        return GridPositionsFree(positions.ToList(), flagsToCheck);
    }

    public GridCell GetOrAddGridCell(Vector2Int position)
    {
        GridCell gridCell = GetGridCell(position);
        if (gridCell != null) return gridCell;
        gridCell = new GridCell();
        occupiedGridCells.Add(position, gridCell);
        return gridCell;
    }

    public GridCell GetGridCell(Vector2Int position)
    {
        if (occupiedGridCells.TryGetValue(position, out GridCell gridCell))
            return gridCell;
        return null;
    }

    public bool OccupyGridField(Vector2Int position, EGridCellOccupiedFlags occupyFlag, IPlaceable occupier)
    {
        if (!GridPositionFree(position, occupyFlag)) return false;
        GridCell gridCell = GetOrAddGridCell(position);
        if (gridCell == null) 
        {
            Dbug.Instance.LogError("<b>OMG THIS SHOULD NOT HAVE HAPPENED WTF</b>");
            return false; 
        }
        gridCell.occupiedFlag |= occupyFlag;
        occupiedGridCells.AddOrReplace(position, gridCell);

        for (int i = 0; i < (int)EGridCellOccupiedFlags.Count; i++)
        {
            EGridCellOccupiedFlags checkedFlag = (EGridCellOccupiedFlags)i;
            if (occupyFlag == checkedFlag)
                gridCell.occupiers.Add(checkedFlag, occupier);
        }
        onGridCellChanged?.Invoke(position);
        return true;
    }


    public bool OccupyGridFields(List<Vector2Int> positions, EGridCellOccupiedFlags occupyFlags, IPlaceable occupier)
    {
        bool success = GridPositionsFree(positions, occupyFlags);
        if (!success) return false;
        foreach (Vector2Int position in positions)
        {
            OccupyGridField(position, occupyFlags, occupier);
        }
        return true;
    }

    public bool OccupyGridFields(Vector2Int[] positions, EGridCellOccupiedFlags occupyFlags, IPlaceable occupier)
    {
        return OccupyGridFields(positions.ToList(), occupyFlags, occupier);
    }

    public bool UnoccupyGridField(Vector2Int position, EGridCellOccupiedFlags unoccupyFlag = EGridCellOccupiedFlags.None)
    {
        if (occupiedGridCells.TryGetValue(position, out GridCell found))
        {
            found.occupiedFlag = EGridCellOccupiedFlags.None;
            for (int i = 0; i < (int)EGridCellOccupiedFlags.Count; i++)
            {
                EGridCellOccupiedFlags checkedFlag = (EGridCellOccupiedFlags)i;
                if (unoccupyFlag == checkedFlag)
                    found.occupiers.Remove(checkedFlag);
            }
            if (found.occupiedFlag == EGridCellOccupiedFlags.None)
                occupiedGridCells.Remove(position);
            onGridCellChanged?.Invoke(position);
            return true;
        }
        return false;
    }

    public bool UnoccupyGridField(Vector2Int position, CardContainer cardToCheck)
    {
        if (occupiedGridCells.TryGetValue(position, out GridCell cell))
        {
            CardContainer found = cell.occupiers[EGridCellOccupiedFlags.Card] as CardContainer;
            if (found != null && found == cardToCheck)
                return UnoccupyGridField(position, EGridCellOccupiedFlags.Card);
        }
        return false;
    }   

    public bool UnoccupyGridFields(List<Vector2Int> positions, EGridCellOccupiedFlags unoccupyFlags = EGridCellOccupiedFlags.None)
    {
        bool success = true;
        for (int i = 0; i < positions.Count; i++)
        {
            if (!UnoccupyGridField(positions[i], unoccupyFlags))
                success = false;
        }
        return success;
    }

    public bool UnoccupyGridFields(Vector2Int[] positions, EGridCellOccupiedFlags unoccupyFlags = EGridCellOccupiedFlags.None)
    {
        return UnoccupyGridFields(positions.ToList(), unoccupyFlags);
    }

    public List<IPlaceable> GetPlaceablesAtPosition(Vector2Int position)
    {
        List<IPlaceable> placeables = new();
        GridCell gridCell = GetGridCell(position);
        foreach (var item in gridCell.occupiers)
        {
            placeables.Add(item.Value);
        }
        return (placeables.Count > 0) ? placeables : null;
    }

    public IPlaceable GetPlaceableAtPosition(Vector2Int position, EGridCellOccupiedFlags flagToCheck)
    {
        GridCell gridCell = GetGridCell(position);
        if (gridCell == null) return null;
        return gridCell.occupiers.ContainsKey(flagToCheck) ? gridCell.occupiers[flagToCheck] : null;
    }

    public float GetWeightOfGridCell(Vector2Int position)
    {
        GridCell cell = GetGridCell(position);
        if (cell == null) return 1f;
        return cell.weight;
    }

    public void InteractPlaceable(Vector3 position, EGridCellOccupiedFlags flagToCheck = EGridCellOccupiedFlags.Card)
    {
        Vector2Int pos = GetGridCoordsAtWorldPosition(position);
        InteractPlaceable(pos, flagToCheck);
    }

    public void InteractPlaceable(Vector2Int position, EGridCellOccupiedFlags flagToCheck = EGridCellOccupiedFlags.Card)
    {
        IPlaceable placeable = GetPlaceableAtPosition(position, flagToCheck);
        placeable?.OnInteract();
    }

    public void ExecutePlaceable(Vector2Int position, EGridCellOccupiedFlags flagToCheck = EGridCellOccupiedFlags.Card)
    {
        IPlaceable placeable = GetPlaceableAtPosition(position, flagToCheck);
        placeable?.OnExecute();
    }

    public void ExecutePlaceable(Vector3 position, EGridCellOccupiedFlags flagToCheck = EGridCellOccupiedFlags.Card)
    {
        Vector2Int pos = GetGridCoordsAtWorldPosition(position);
        ExecutePlaceable(pos, flagToCheck);
    }

    public bool WithinGridPlayArea(Vector2Int position)
    {
        return position.x >= 0 && position.x < GridWidth && position.y >= 0 && position.y < GridHeight;
    }

    public Vector2Int ClampPositionToPlayerGrid(Vector2Int position)
    {
        int x = Mathf.Clamp(position.x, 0, GridWidth - 1);
        int y = Mathf.Clamp(position.y, 0, GridHeight - EnemyRows - 1);
        return new Vector2Int(x, y);
    }

    public void MoveSelection(Vector2Int position)
    {
        position = ClampPositionToPlayerGrid(position);
        selection.transform.position = GetGridCellPosition(position);
        onSelectionGridPositionChanged?.Invoke(position);
    }

    public void MoveSelection(Vector3 position)
    {
        Vector2Int pos = GetGridCoordsAtWorldPosition(position);
        MoveSelection(pos);
    }
}
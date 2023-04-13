using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public interface IPlaceable
{
    Vector2Int Size { get; }
    Vector2Int GridPosition { get; }
    void OnPlace();
    void OnKill();
    void OnInteract();
    void OnSpawn();
}

[Flags]
public enum EGridCellOccupiedFlags
{
    None = 0,
    Enemy = 0x1,
    Unselectable = 0x2,
    Card = 0x4,
    //SecondStory = 0x8,
    // = 0x10
    // = 0x20
    // etc
    All = 0b111111111111
}
//0b0000001111

public class GridCell
{
    public EGridCellOccupiedFlags occupiedFlags = EGridCellOccupiedFlags.None;
    public Dictionary<EGridCellOccupiedFlags, IPlaceable> occupiers = new Dictionary<EGridCellOccupiedFlags, IPlaceable>();
    public float weight = 1f;
}

public class GridManager : MonoBehaviour
{
    public static GridManager Instance;

    public delegate void OnSelectionPositionChanged(Vector2Int newPosition);
    public static event OnSelectionPositionChanged onSelectionPositionChanged;
    public delegate void OnGridCellChanged(Vector2Int cellPosition);
    public static event OnGridCellChanged onGridCellChanged;

    public static EGridCellOccupiedFlags playerBlocked = EGridCellOccupiedFlags.Unselectable;

    public Vector3 SelectionPositionWorld { get { return selection.transform.position + new Vector3(gridHorizontalSize / 2f, 0f, gridVerticalSize / 2f); } }
    public Vector2Int SelectionPositionGrid { get { return currentSelectionCoords; } }
    public IPlaceable SelectedPlaceable { get; private set; }

    // NEED TO MANUALLY CHANGE //
    public int GridWidth { get { return 12; } }
    public int GridHeight { get { return 7; } }
    public int EnemyRows { get { return 3; } }
    /////////////////////////////

    public float gridVerticalSize = 0.2f;
    public float gridHorizontalSize = 0.1f;
    public GameObject selection;
    public LayerMask groundLayer;

    private Vector2Int currentSelectionCoords = -Vector2Int.one;
    private Dictionary<Vector2Int, GridCell> occupiedGridCells = new Dictionary<Vector2Int, GridCell>();

    private void Start()
    {
        Instance = this;
        selection = Instantiate(selection);
        selection.transform.localScale = new Vector3(gridHorizontalSize, 0.01f, gridVerticalSize);

        for (int x = 0; x < GridWidth; x++)
        {
            for (int y = GridHeight - EnemyRows; y < GridHeight; y++)
            {
                OccupyGridField(new Vector2Int(x, y), playerBlocked, null);
            }
        }
    }

    private void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 99999f, groundLayer))
        {
            Vector2Int coords = GetGridCoordsAtWorldPosition(hit.point);
            coords = ClampPositionToPlayerGrid(coords);

            if (coords == currentSelectionCoords) return;
            currentSelectionCoords = coords;
            Vector3 selectionCellPosition = GetGridCellPosition(coords);
            selection.transform.position = selectionCellPosition;
            onSelectionPositionChanged?.Invoke(coords);
        }
    }

    public Vector2Int GetGridCoordsAtWorldPosition(Vector3 worldPosition)
    {
        Vector3 relativePosition = worldPosition - transform.position;
        Vector3 gridPosition = Vector3.zero;
        gridPosition.x = relativePosition.x / gridHorizontalSize;
        gridPosition.z = relativePosition.z / gridVerticalSize;
        return new Vector2Int(Mathf.FloorToInt(gridPosition.x), Mathf.FloorToInt(gridPosition.z));
    }

    public Vector3 GetGridCellPosition(Vector2Int gridPosition)
    {
        return transform.position + new Vector3(gridPosition.x * gridHorizontalSize, 0.0f, gridPosition.y * gridVerticalSize);
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

    public bool GridPositionFree(Vector2Int position, EGridCellOccupiedFlags flagsToCheck = EGridCellOccupiedFlags.All)
    {
        if (occupiedGridCells.TryGetValue(position, out GridCell gridCell))
        {
            return !gridCell.occupiedFlags.HasFlag(flagsToCheck);
        }
        return true;
    }

    public bool GridPositionsFree(List<Vector2Int> positions, EGridCellOccupiedFlags flagsToCheck = EGridCellOccupiedFlags.All)
    {
        foreach(Vector2Int position in positions)
        {
            if (!GridPositionFree(position, flagsToCheck))
                return false;
        }
        return true;
    }

    public bool GridPositionsFree(Vector2Int[] positions, EGridCellOccupiedFlags flagsToCheck = EGridCellOccupiedFlags.All)
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

    public bool OccupyGridField(Vector2Int position, EGridCellOccupiedFlags occupyFlags, IPlaceable occupier)
    {
        if (!GridPositionFree(position, occupyFlags)) return false;
        GridCell gridCell = GetOrAddGridCell(position);
        if (gridCell == null) 
        {
            Dbug.Instance.LogError("<b>OMG THIS SHOULD NOT HAVE HAPPENED WTF</b>");
            return false; 
        }
        gridCell.occupiedFlags |= occupyFlags;

        for (int i = 0; i < 32; i++)
        {
            EGridCellOccupiedFlags checkedFlag = (EGridCellOccupiedFlags)(1 << i);
            if (occupyFlags.HasFlag(checkedFlag))
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

    public bool UnoccupyGridField(Vector2Int position, EGridCellOccupiedFlags unoccupyFlags = EGridCellOccupiedFlags.All)
    {
        if (occupiedGridCells.TryGetValue(position, out GridCell found))
        {
            found.occupiedFlags &= ~unoccupyFlags;
            for (int i = 0; i < 32; i++)
            {
                EGridCellOccupiedFlags checkedFlag = (EGridCellOccupiedFlags)(1 << i);
                if (unoccupyFlags.HasFlag(checkedFlag))
                    found.occupiers.Remove(checkedFlag);
            }
            if (found.occupiedFlags == EGridCellOccupiedFlags.None)
                occupiedGridCells.Remove(position);
            onGridCellChanged?.Invoke(position);
            return true;
        }
        return false;
    }

    public bool UnoccupyGridFields(List<Vector2Int> positions, EGridCellOccupiedFlags unoccupyFlags = EGridCellOccupiedFlags.All)
    {
        bool success = true;
        for (int i = 0; i < positions.Count; i++)
        {
            if (!UnoccupyGridField(positions[i], unoccupyFlags))
                success = false;
        }
        return success;
    }

    public bool UnoccupyGridFields(Vector2Int[] positions, EGridCellOccupiedFlags unoccupyFlags = EGridCellOccupiedFlags.All)
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

    public bool IsPlayableArea(Vector2Int position)
    {
        GridCell cell = GetGridCell(position);
        if (cell == null) return true;
        return (cell.occupiedFlags & EGridCellOccupiedFlags.Unselectable) != EGridCellOccupiedFlags.Unselectable;
    }

    public void InteractWithPlaceable(Vector2Int position, EGridCellOccupiedFlags flagToCheck = EGridCellOccupiedFlags.Card)
    {
        IPlaceable placeable = GetPlaceableAtPosition(position, flagToCheck);
        placeable?.OnInteract();
    }

    public void InteractWithPlaceable(Vector3 position, EGridCellOccupiedFlags flagToCheck = EGridCellOccupiedFlags.Card)
    {
        Vector2Int pos = GetGridCoordsAtWorldPosition(position);
        InteractWithPlaceable(pos, flagToCheck);
    }

    public bool WithinGridPlayArea(Vector2Int position)
    {
        return position.x >= 0 && position.x < GridWidth && position.y >= 0 && position.y < GridHeight;
    }

    public Vector2Int ClampPositionToPlayerGrid(Vector2Int position)
    {
        int x = Mathf.Clamp(position.x, 0, GridWidth);
        int y = Mathf.Clamp(position.y, 0, GridHeight - EnemyRows - 1);
        return new Vector2Int(x, y);
    }
}
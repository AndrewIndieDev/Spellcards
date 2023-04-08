using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public interface IPlaceable
{
    GameObject gameObject { get; }
    bool UserCanDemolish { get; }
    bool UserCanInteract { get; }
    Vector2Int Size { get; }
    Vector2Int GridPosition { get; }
    void OnPlace();
    void OnDemolish();
    void OnInteract();
}

[Flags]
public enum EGridCellOccupiedFlags
{
    None = 0,
    Enemy = 0x1,
    Walkable = 0x2,
    //Pipe = 0x4,
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


    public static EGridCellOccupiedFlags playerBlocked = EGridCellOccupiedFlags.Enemy;

    public Vector3 SelectionPositionWorld { get { return selection.transform.position; } }
    public Vector2Int SelectionPositionGrid { get { return currentSelectionCoords; } }
    public IPlaceable SelectedPlaceable { get; private set; }

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
    }

    private void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 99999f, groundLayer))
        {
            Vector2Int coords = GetGridCoordsAtWorldPosition(hit.point);
            SelectedPlaceable = GetBuildingAtPosition(coords);
            if (coords == currentSelectionCoords) return;
            currentSelectionCoords = coords;
            Vector3 selectionCellPosition = GetGridCellPosition(coords);
            selection.transform.position = selectionCellPosition;
            onSelectionPositionChanged?.Invoke(coords);
            occupiedGridCells.TryGetValue(coords, out GridCell gridCell);
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
        GridCell gridCell;
        if (occupiedGridCells.TryGetValue(position, out gridCell))
        {
            return !flagsToCheck.HasFlag(gridCell.occupiedFlags);
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

    public bool OccupyGridField(Vector2Int position, EGridCellOccupiedFlags occupyFlags = EGridCellOccupiedFlags.All, IPlaceable occupier = null)
    {
        if (!GridPositionFree(position, occupyFlags)) return false;
        GridCell gridCell = GetOrAddGridCell(position);
        if (gridCell == null) 
        {
            Debug.LogError("<b>OMG THIS SHOULD NOT HAVE HAPPENED WTF</b>");
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


    public bool OccupyGridFields(List<Vector2Int> positions, EGridCellOccupiedFlags occupyFlags = EGridCellOccupiedFlags.All, IPlaceable occupier = null)
    {
        bool success = GridPositionsFree(positions, occupyFlags);
        if (!success) return false;
        foreach (Vector2Int position in positions)
        {
            OccupyGridField(position, occupyFlags, occupier);
        }
        return true;
    }

    public bool OccupyGridFields(Vector2Int[] positions, EGridCellOccupiedFlags occupyFlags = EGridCellOccupiedFlags.All, IPlaceable occupier = null)
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

    public IPlaceable GetBuildingAtPosition(Vector2Int position)
    {
        GridCell gridCell = GetGridCell(position);
        return gridCell != null ? gridCell.occupiers[EGridCellOccupiedFlags.Enemy] : null;
    }

    public float GetWeightOfGridCell(Vector2Int position)
    {
        GridCell cell = GetGridCell(position);
        if (cell == null) return 1f;
        return cell.weight;
    }

    public bool GetIsWalkable(Vector2Int position)
    {
        GridCell cell = GetGridCell(position);
        if (cell == null) return true;
        return (cell.occupiedFlags & EGridCellOccupiedFlags.Walkable) == EGridCellOccupiedFlags.Walkable;
    }
}
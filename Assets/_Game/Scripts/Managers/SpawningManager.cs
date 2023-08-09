using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;

public class SpawningManager : MonoBehaviour
{
    public static SpawningManager Instance;

    private GridManager Grid => GridManager.Instance;

    [Title("Inspector References")]
    [SerializeField] private CardContainer r_CardPrefab;
    [SerializeField] private CardData r_CraftingCardData;

    #region Unity Methods
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    #endregion

    #region Public Methods
    
    /// <summary>
    /// Spawns an enemy at a random position in the enemy rows.
    /// </summary>
    /// <param name="data">Data you want the card to spawn with.</param>
    public CardContainer SpawnEnemy(CardData data)
    {
        int rowMin = Grid.GridHeight - Grid.EnemyRows;
        int rowMax = Grid.GridHeight;
        var gridPosition = GetRandomGridPosition(rowMin, rowMax, 0, Grid.GridWidth);

        var attemptsToPlaceCard = 100;
        while (!Grid.GridPositionFree(gridPosition, EGridCellOccupiedFlags.Card) && attemptsToPlaceCard > 0)
        {
            gridPosition = GetRandomGridPosition(rowMin, rowMax, 0, Grid.GridWidth);
            attemptsToPlaceCard--;

            if (attemptsToPlaceCard == 0)
            {
                Dbug.Instance.LogError($"Couldn't find a position to place {data.name}.");
                return null;
            }
        }
        return SpawnCard(data, gridPosition);
    }
    /// <summary>
    /// Spawns a card at the desired grid position using the given world position.
    /// </summary>
    /// <param name="data">Data you want the card to spawn with.</param>
    /// <param name="worldPosition"></param>
    public CardContainer SpawnCard(CardData data, Vector3 worldPosition = default)
    {
        if (data.GetType() == typeof(EnemyCard))
        {
            return SpawnEnemy(data);
        }

        var gridPosition = Vector2Int.zero;
        if (worldPosition == default)
        {
            gridPosition = GetRandomGridPosition(0, Grid.GridHeight - Grid.EnemyRows, 0, Grid.GridWidth);

            var attemptsToPlaceCard = 200;
            while (!Grid.GridPositionFree(gridPosition, EGridCellOccupiedFlags.Card) && attemptsToPlaceCard > 0)
            {
                gridPosition = GetRandomGridPosition(0, Grid.GridHeight - Grid.EnemyRows, 0, Grid.GridWidth);
                attemptsToPlaceCard--;

                if (attemptsToPlaceCard == 0)
                {
                    Dbug.Instance.LogError($"Couldn't find a position to place {data.name}.");
                    return null;
                }
            }
        }
        return SpawnCard(data, gridPosition);
    }
    /// <summary>
    /// Spawns a card at the desired grid position.
    /// </summary>
    /// <param name="data">Data you want the card to spawn with.</param>
    /// <param name="gridPosition">Grid position you want to spawn the card at.</param>
    public CardContainer SpawnCard(CardData data, Vector2Int gridPosition)
    {
        List<Vector2Int> toOccupy = new();
        for (int x = 0; x < data.xSize; x++)
        {
            for (int y = 0; y < data.ySize; y++)
            {
                toOccupy.Add(new Vector2Int(gridPosition.x + x, gridPosition.y + y));
            }
        }
        if (!Grid.GridPositionsFree(toOccupy))
            return null;
        var cell = Grid.GetOrAddGridCell(gridPosition);
        if (cell == null)
        {
            Dbug.Instance.LogError($"Cell [{gridPosition.x},{gridPosition.y}] is null!");
            return null;
        }
        CardContainer card = Instantiate(r_CardPrefab, Vector3.zero, Quaternion.identity);
        card.MoveInstant(Grid.GetGridCellCenterPosition(gridPosition));
        card.SetData(data);
        Grid.OccupyGridFields(toOccupy, EGridCellOccupiedFlags.Card, card);
        card.OnSpawn();
        return card;
    }

    public CardContainer SpawnCrafting(Vector2Int gridPosition, float craftTime, System.Action callback)
    {
        List<Vector2Int> toOccupy = new();
        for (int x = 0; x < r_CraftingCardData.xSize; x++)
        {
            for (int y = 0; y < r_CraftingCardData.ySize; y++)
            {
                toOccupy.Add(new Vector2Int(gridPosition.x + x, gridPosition.y + y));
            }
        }
        if (!Grid.GridPositionsFree(toOccupy))
            return null;
        var cell = Grid.GetOrAddGridCell(gridPosition);
        if (cell == null)
        {
            Dbug.Instance.LogError($"Cell [{gridPosition.x},{gridPosition.y}] is null!");
            return null;
        }
        CardContainer card = Instantiate(r_CardPrefab, Vector3.zero, Quaternion.identity);
        card.MoveInstant(Grid.GetGridCellCenterPosition(gridPosition));
        card.SetData(r_CraftingCardData);
        Grid.OccupyGridFields(toOccupy, EGridCellOccupiedFlags.Card, card);
        card.OnSpawn();
        card.Timer.Run(craftTime, callback);
        return card;
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Get's a random grid position within the parameters.
    /// </summary>
    /// <param name="minRow">Minumum Row. (inclusive)</param>
    /// <param name="maxRow">Maximum Row. (exclusive)</param>
    /// <param name="minCol">Minumum Column. (inclusive)</param>
    /// <param name="maxCol">Maximum Column. (exclusive)</param>
    /// <returns></returns>
    private Vector2Int GetRandomGridPosition(int minRow, int maxRow, int minCol, int maxCol)
    {
        int colToSpawn = Random.Range(minCol, maxCol);
        int rowToSpawn = Random.Range(minRow, maxRow);
        return new Vector2Int(colToSpawn, rowToSpawn);
    }
    #endregion
}

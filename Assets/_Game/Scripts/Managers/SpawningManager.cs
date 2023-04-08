using UnityEngine;
using Sirenix.OdinInspector;

public class SpawningManager : MonoBehaviour
{
    public static SpawningManager Instance;
    private void Awake()
    {
        Instance = this;
    }

    private GridManager Grid => GridManager.Instance;

    [Title("Inspector References")]
    [SerializeField] private CardContainer r_CardPrefab;

    public void SpawnCard(CardData data, Vector3 position)
    {
        var pos = new Vector2Int(Mathf.FloorToInt(position.x), Mathf.FloorToInt(position.z));
        var cell = Grid.GetGridCell(pos);
        if (cell == null)
        {
            Dbug.Instance.LogError("Cell is null!");
            return;
        }
        CardContainer card = Instantiate(r_CardPrefab, Vector3.zero, Quaternion.identity);
        card.SetData(data);
        card.MoveInstant(position);
    }
    public void SpawnEnemy(CardData data)
    {
        int rowMin = Grid.gridHeight - Grid.enemyRows;
        int rowMax = Grid.gridHeight;
        int rowToSpawn = Random.Range(rowMin, rowMax);
        int colToSpawn = Random.Range(0, Grid.gridWidth);
        var pos = new Vector2Int(colToSpawn, rowToSpawn);
        var cell = Grid.GetGridCell(pos);
        if (cell == null)
        {
            Dbug.Instance.LogError("Cell is null!");
            return;
        }
        var cellPos = Grid.GetGridCellCenterPosition(pos);
        CardContainer card = Instantiate(r_CardPrefab, Vector3.zero, Quaternion.identity);
        card.SetData(data);
        card.MoveInstant(cellPos);
    }
}

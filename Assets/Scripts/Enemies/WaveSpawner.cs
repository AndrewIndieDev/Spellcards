using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    public static WaveSpawner Instance;
    private void Awake() => Instance = this;

    [Title("Fields")]
    [SerializeField] private float secondsBetweenSpawns;
    [SerializeField] private float bossEveryXWaves;

    [Title("References")]
    [SerializeField] private EnemyGroup enemyGroupPrefab;
    [SerializeField] private EnemyGroup enemyBossPrefab;
    [SerializeField] private Transform enemyEndTransform;

    private List<EnemyGroup> enemyGroupList = new();
    private int wavesSinceStart;

    private void Start()
    {
        GameManager.Instance.OnGameStart += OnGameStart;
        GameManager.Instance.OnGameEnd += OnGameEnd;
        GameManager.Instance.OnEnemyGroupDies += OnEnemyGroupDies;
    }

    private void OnEnemyGroupDies(EnemyGroup enemyGroup)
    {
        if (enemyGroupList.Contains(enemyGroup))
            enemyGroupList.Remove(enemyGroup);
    }

    private void OnGameStart()
    {
        StartCoroutine(SpawnWaves());
    }

    private void OnGameEnd()
    {
        enemyGroupList.Clear();
    }

    public void Spawn()
    {
        wavesSinceStart++;
        EnemyGroup eg = Instantiate((wavesSinceStart % bossEveryXWaves == 0) ? enemyBossPrefab : enemyGroupPrefab, transform.position, Quaternion.identity);
        enemyGroupList.Add(eg);
    }

    public Vector3 GetStartPosition => transform.position;
    public Vector3 GetEndPosition => enemyEndTransform.position;
    public EnemyGroup GetClosestEnemyGroup => enemyGroupList[0];

    private IEnumerator SpawnWaves()
    {
        yield return null;
        while (GameManager.Instance.playing)
        {
            Spawn();
            yield return new WaitForSeconds(secondsBetweenSpawns);
        }
    }
}

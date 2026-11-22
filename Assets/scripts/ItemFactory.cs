using System.Collections.Generic;
using UnityEngine;

public class ItemFactory : MonoBehaviour
{
    [Header("Prefab 與九宮格座標")]
    public GameObject applePrefab;
    public GameObject bombPrefab;
    public float[] gridCoords = new float[] { -1f, 0f, 1f };
    public float spawnY = 8f;

    [Header("流程關聯")]
    public GameDirector director;

    [Header("第一階段（90s ~ 70s，共 20s）")]
    public float stage1Interval = 0.9f;
    public float stage1Speed = 3.5f;
    [Range(0f, 1f)] public float stage1BombRate = 0.1f;

    [Header("第二階段（70s ~ 20s，共 50s）")]
    public float stage2Interval = 0.45f;
    public float stage2Speed = 6.0f;
    [Range(0f, 1f)] public float stage2BombRate = 0.4f;

    [Header("第三階段（20s ~ 0s，共 20s）")]
    public float stage3Interval = 0.25f;
    public float stage3Speed = 9.5f;
    [Range(0f, 1f)] public float stage3BombRate = 1.0f;

    private float spawnTimer = 0f;
    private Vector2Int lastSpawnCell = new Vector2Int(-1, -1);

    private void Update()
    {
        if (director == null || director.IsGameOver)
            return;

        GetCurrentStageParams(director.RemainingTime, out float interval, out float speed, out float bombRate);

        spawnTimer += Time.deltaTime;
        if (spawnTimer >= interval)
        {
            spawnTimer = 0f;
            SpawnItem(speed, bombRate);
        }
    }

    private void GetCurrentStageParams(float remainingTime, out float interval, out float speed, out float bombRate)
    {
        if (remainingTime > 70f)
        {
            interval = stage1Interval;
            speed = stage1Speed;
            bombRate = stage1BombRate;
        }
        else if (remainingTime > 20f)
        {
            interval = stage2Interval;
            speed = stage2Speed;
            bombRate = stage2BombRate;
        }
        else
        {
            interval = stage3Interval;
            speed = stage3Speed;
            bombRate = stage3BombRate;
        }
    }

    private void SpawnItem(float speed, float bombRate)
    {
        if (gridCoords == null || gridCoords.Length == 0)
            return;

        // 收集所有九宮格座標索引，盡量避免連續兩次掉在同一格
        List<Vector2Int> candidates = new List<Vector2Int>();
        for (int x = 0; x < gridCoords.Length; x++)
        {
            for (int z = 0; z < gridCoords.Length; z++)
            {
                Vector2Int cell = new Vector2Int(x, z);
                if (cell != lastSpawnCell)
                {
                    candidates.Add(cell);
                }
            }
        }

        Vector2Int chosenCell = candidates[Random.Range(0, candidates.Count)];
        lastSpawnCell = chosenCell;

        Vector3 spawnPos = new Vector3(
            gridCoords[chosenCell.x],
            spawnY,
            gridCoords[chosenCell.y]
        );

        bool spawnBomb = Random.value < bombRate;
        GameObject prefabToSpawn = spawnBomb ? bombPrefab : applePrefab;

        if (prefabToSpawn == null)
            return;

        GameObject instance = Instantiate(prefabToSpawn, spawnPos, Quaternion.identity);
        FallingItem fallingItem = instance.GetComponent<FallingItem>();
        if (fallingItem != null)
        {
            fallingItem.Initialize(speed, director);
        }
    }
}
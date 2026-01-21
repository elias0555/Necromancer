using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ContinuousSpawnManager : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private List<EnemyData> allEnemies;

    [Header("Spawn Settings")]
    [SerializeField] private Transform centerTarget;
    [SerializeField] private float spawnRadius = 15f;
    [SerializeField] private int maxActiveEnemies = 50;

    [Header("Timing & Difficulty")]
    [SerializeField] private float timeBetweenSpawns = 1f;
    [SerializeField] private float minTimeBetweenSpawns = 0.1f;
    [SerializeField] private float difficultyScalingFactor = 0.05f;

    [SerializeField] private float levelUpDuration = 30f;

    [Header("Boss Settings")]
    [SerializeField] private GameObject bossPrefab;
    [SerializeField] private float bossSpawnTimer = 60f;
    private float currentBossTimer = 0f;

    private float globalTimer = 0f;
    private float spawnTimer = 0f;
    private float currentDifficultyMultiplier = 1f;
    private int currentDifficultyLevel = 1;

    private List<GameObject> activeEnemies = new List<GameObject>();

    void Start()
    {
        if (centerTarget == null) centerTarget = transform;
    }

    void Update()
    {
        globalTimer += Time.deltaTime;
        CalculateDifficulty();

        activeEnemies.RemoveAll(x => x == null);

        HandleBossSpawning();

        if (activeEnemies.Count < maxActiveEnemies)
        {
            HandleNormalSpawning();
        }
    }

    void CalculateDifficulty()
    {
        int newLevel = 1 + (int)(globalTimer / levelUpDuration);

        if (newLevel > currentDifficultyLevel)
        {
            currentDifficultyLevel = newLevel;
            Debug.Log($"Niveau de difficulté augmenté : {currentDifficultyLevel}");

            timeBetweenSpawns = Mathf.Max(minTimeBetweenSpawns, timeBetweenSpawns * 0.95f);
        }

        currentDifficultyMultiplier = 1f + (globalTimer * difficultyScalingFactor / 60f);
    }

    void HandleNormalSpawning()
    {
        spawnTimer += Time.deltaTime;

        if (spawnTimer >= timeBetweenSpawns)
        {
            spawnTimer = 0f;

            List<EnemyData> validEnemies = allEnemies.Where(e => e.minWaveToSpawn <= currentDifficultyLevel).ToList();

            if (validEnemies.Count > 0)
            {
                EnemyData data = GetWeightedRandomEnemy(validEnemies);
                SpawnEnemy(data);
            }
        }
    }

    void HandleBossSpawning()
    {
        currentBossTimer += Time.deltaTime;
        if (currentBossTimer >= bossSpawnTimer)
        {
            currentBossTimer = 0f;
            SpawnBoss();
        }
    }

    void SpawnEnemy(EnemyData data)
    {
        Vector3 spawnPos = GetRandomPositionOnCircle();
        GameObject newEnemy = Instantiate(data.prefab, spawnPos, Quaternion.identity);

        if (newEnemy.TryGetComponent(out BaseEnemy enemyScript))
        {
            if (newEnemy.TryGetComponent(out HealthComponent hp))
            {
                hp.SetMaxHealth(data.baseHealth); 
            }

            enemyScript.Initialize(currentDifficultyMultiplier, currentDifficultyMultiplier);
        }
        activeEnemies.Add(newEnemy);
    }

    void SpawnBoss()
    {
        Debug.Log("--- BOSS APPEARED ---");
        Vector3 spawnPos = GetRandomPositionOnCircle();
        GameObject boss = Instantiate(bossPrefab, spawnPos, Quaternion.identity);

        if (boss.TryGetComponent(out BaseEnemy enemyScript))
        {
            enemyScript.Initialize(currentDifficultyMultiplier * 3f, currentDifficultyMultiplier * 1.5f);
        }
        activeEnemies.Add(boss);
    }

    Vector3 GetRandomPositionOnCircle()
    {
        Vector2 randomPoint = Random.insideUnitCircle.normalized * spawnRadius;
        Vector3 spawnPos = centerTarget.position + (Vector3)randomPoint;
        spawnPos.z = centerTarget.position.z;
        return spawnPos;
    }

    EnemyData GetWeightedRandomEnemy(List<EnemyData> enemies)
    {
        float totalWeight = enemies.Sum(e => e.spawnWeight);
        float randomValue = Random.Range(0, totalWeight);
        float cursor = 0;
        foreach (var enemy in enemies)
        {
            cursor += enemy.spawnWeight;
            if (cursor >= randomValue) return enemy;
        }
        return enemies[0];
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector3 center = centerTarget != null ? centerTarget.position : transform.position;
        Gizmos.DrawWireSphere(center, spawnRadius);
    }
}
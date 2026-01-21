using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ContinuousSpawnManager : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private List<UnitProfileSO> allEnemies; // Directement les profils !
    [SerializeField] private GameObject genericEnemyPrefab;

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

            // Filtre directement sur les profils
            List<UnitProfileSO> validProfiles = allEnemies
                .Where(p => p.minWaveToSpawn <= currentDifficultyLevel)
                .ToList();

            if (validProfiles.Count > 0)
            {
                UnitProfileSO chosenProfile = GetWeightedRandomProfile(validProfiles);
                SpawnEnemy(chosenProfile);
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

    void SpawnEnemy(UnitProfileSO profile)
    {
        Vector3 spawnPos = GetRandomPositionOnCircle();

        GameObject newEnemy = Instantiate(genericEnemyPrefab, spawnPos, Quaternion.identity);

        // 1. Configurer le corps (Vitesse, Sprite)
        if (newEnemy.TryGetComponent(out UnitController unit))
        {
            unit.Initialize(profile);
        }

        // 2. Configurer le cerveau (Pour le drop du cadavre)
        if (newEnemy.TryGetComponent(out EnemyBrain brain))
        {
            brain.SetProfile(profile);
        }

        // 3. Appliquer la difficulté sur les PV du profil
        if (newEnemy.TryGetComponent(out HealthComponent hp))
        {
            float scaledHealth = profile.maxHealth * currentDifficultyMultiplier;
            hp.SetMaxHealth(scaledHealth);
        }

        activeEnemies.Add(newEnemy);
    }

    void SpawnBoss()
    {
        Vector3 spawnPos = GetRandomPositionOnCircle();
        GameObject boss = Instantiate(bossPrefab, spawnPos, Quaternion.identity);

        if (boss.TryGetComponent(out HealthComponent hp))
        {
            hp.SetMaxHealth(2000f * currentDifficultyMultiplier);
        }

        activeEnemies.Add(boss);
    }

    Vector3 GetRandomPositionOnCircle()
    {
        Vector2 randomPoint = Random.insideUnitCircle.normalized * spawnRadius;
        return centerTarget.position + (Vector3)randomPoint;
    }

    UnitProfileSO GetWeightedRandomProfile(List<UnitProfileSO> profiles)
    {
        float totalWeight = profiles.Sum(p => p.spawnWeight);
        float randomValue = Random.Range(0, totalWeight);
        float cursor = 0;
        foreach (var profile in profiles)
        {
            cursor += profile.spawnWeight;
            if (cursor >= randomValue) return profile;
        }
        return profiles[0];
    }
}
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ContinuousSpawnManager : MonoBehaviour
{
    [Header("Configuration")]
    

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

	void Awake()
	{
		
	}

	void Update()
    {
        
    }

    Vector3 GetRandomPositionOnCircle()
    {
        Vector2 randomPoint = Random.insideUnitCircle.normalized * spawnRadius;
        return centerTarget.position + (Vector3)randomPoint;
    }
}
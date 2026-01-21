using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyData", menuName = "WaveSystem/EnemyData")]
public class EnemyData : ScriptableObject
{
    public string enemyName;
    public GameObject prefab;

    [Header("Spawn Rules")]
    public int minWaveToSpawn = 1; 
    public float spawnWeight = 1f; 

    [Header("Base Stats")]
    public float baseHealth = 100f;
    public int baseDamage = 10;
}
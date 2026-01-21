using UnityEngine;

[CreateAssetMenu(fileName = "NewUnitProfile", menuName = "Necro/UnitProfile")]
public class UnitProfileSO : ScriptableObject
{
    [Header("Identité")]
    public string unitName;
    public Sprite visualSprite;
    public Sprite corpseSprite;

    [Header("Stats de Combat")]
    public float maxHealth = 100f;
    public float moveSpeed = 3f;
    public GameObject abilityPrefab; // L'arme (Script ICombatAbility)

    [Header("Règles d'Apparition (Wave Manager)")]
    public int minWaveToSpawn = 1;
    public float spawnWeight = 1f; // Chance d'apparition relative
}
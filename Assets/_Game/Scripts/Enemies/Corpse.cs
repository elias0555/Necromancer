using UnityEngine;

[RequireComponent(typeof(HealthComponent))]
public class Corpse : MonoBehaviour
{
    [Header("Resurrection Settings")]
    [SerializeField] private GameObject minionVersionPrefab; 
    [SerializeField] private Color corpseColor = Color.gray;

    private bool isDowned = false;
    private HealthComponent health;
    private BaseEnemy enemyAI;
    private Collider2D col;
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        health = GetComponent<HealthComponent>();
        enemyAI = GetComponent<BaseEnemy>();
        col = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void OnEnable()
    {
        health.onDeath.AddListener(BecomeCorpse);
    }

    void OnDisable()
    {
        health.onDeath.RemoveListener(BecomeCorpse);
    }

    private void BecomeCorpse()
    {
        if (isDowned) return;
        isDowned = true;

        if (enemyAI != null) enemyAI.enabled = false;
        if (GetComponent<Rigidbody2D>() != null) GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;

        gameObject.layer = LayerMask.NameToLayer("Corpse");

        transform.rotation = Quaternion.Euler(0, 0, 90);
        if (spriteRenderer != null) spriteRenderer.color = corpseColor;

        if (col != null) col.isTrigger = true;

        Debug.Log($"{name} is now a corpse.");
    }

    public GameObject GetMinionPrefab()
    {
        return minionVersionPrefab;
    }
}
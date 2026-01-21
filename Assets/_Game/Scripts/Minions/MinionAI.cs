using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class MinionAI : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private float moveSpeed = 3.5f;
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private int damage = 5;
    [SerializeField] private float attackCooldown = 1f;

    [Header("Behavior")]
    [SerializeField] private LayerMask enemyLayer;

    // Internal references
    private Transform playerTransform;
    private float detectionRadius;
    private Transform currentTarget;
    private Rigidbody2D rb;
    private Vector2 wanderTarget;
    private float wanderTimer;
    private float lastAttackTime;

    public void Initialize(Transform player, float radius)
    {
        playerTransform = player;
        detectionRadius = radius;
        rb = GetComponent<Rigidbody2D>();
        PickRandomWanderPoint();
    }

    void Update()
    {
        if (playerTransform == null) return;

        if (currentTarget == null)
        {
            FindTarget();
            WanderBehavior();
        }
        else
        {
            CombatBehavior();
        }
    }

    private void FindTarget()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(playerTransform.position, detectionRadius, enemyLayer);

        float closestDistance = Mathf.Infinity;
        Transform potentialTarget = null;

        foreach (var hit in hits)
        {
            float dist = Vector2.Distance(transform.position, hit.transform.position);
            if (dist < closestDistance)
            {
                closestDistance = dist;
                potentialTarget = hit.transform;
            }
        }

        currentTarget = potentialTarget;
    }

    private void WanderBehavior()
    {
        if (Vector2.Distance(transform.position, wanderTarget) < 0.5f || wanderTimer <= 0)
        {
            PickRandomWanderPoint();
            wanderTimer = Random.Range(2f, 4f);
        }
        else
        {
            wanderTimer -= Time.deltaTime;
        }

        MoveTo(wanderTarget);
    }

    private void PickRandomWanderPoint()
    {
        Vector2 randomPoint = Random.insideUnitCircle * detectionRadius;
        wanderTarget = (Vector2)playerTransform.position + randomPoint;
    }

    private void CombatBehavior()
    {
        if (currentTarget == null) return;

        if (!currentTarget.gameObject.activeInHierarchy)
        {
            currentTarget = null;
            return;
        }

        float distanceToTarget = Vector2.Distance(transform.position, currentTarget.position);

        if (distanceToTarget > attackRange)
        {
            MoveTo(currentTarget.position);
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
            if (Time.time >= lastAttackTime + attackCooldown)
            {
                Attack();
            }
        }

        if (Vector2.Distance(playerTransform.position, currentTarget.position) > detectionRadius * 1.5f)
        {
            currentTarget = null;
        }

    }

    private void MoveTo(Vector2 targetPos)
    {
        Vector2 direction = (targetPos - (Vector2)transform.position).normalized;
        rb.linearVelocity = direction * moveSpeed;
    }

    private void Attack()
    {
        lastAttackTime = Time.time;

        if (currentTarget.TryGetComponent(out HealthComponent hp))
        {
            hp.TakeDamage(damage);
        }
    }
}
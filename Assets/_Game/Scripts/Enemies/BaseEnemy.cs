using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class BaseEnemy : MonoBehaviour
{
    [Header("Movement Stats")]
    [SerializeField] protected float moveSpeed = 3f;
    [SerializeField] protected float recoilForce = 5f;
    [SerializeField] protected float recoilDuration = 0.5f;
    [Header("Combat Stats")]
    [SerializeField] protected int damage = 10;
    [SerializeField] protected float attackCooldown = 1f;

    protected HealthComponent health;
    protected Transform player;
    protected Rigidbody2D rb;

    protected bool isRecoiling = false;
    protected float lastAttackTime = -999f;

    protected virtual void Awake()
    {
        health = GetComponent<HealthComponent>();
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    protected virtual void FixedUpdate()
    {
        if (player != null)
        {
            HandleSpriteFlip();

            if (!isRecoiling)
            {
                MoveTowardsPlayer();
            }
        }
    }

    protected virtual void HandleSpriteFlip()
    {
        if (transform.position.x < player.position.x)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
    }

    public virtual void Initialize(float healthMultiplier, float damageMultiplier)
    {
        if (health != null)
        {
            float newMaxHealth = health.GetCurrentHealth() * healthMultiplier;
            health.SetMaxHealth(newMaxHealth);
        }

        damage = Mathf.RoundToInt(damage * damageMultiplier);
    }

    protected virtual void MoveTowardsPlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        Vector2 newPos = rb.position + direction * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(newPos);
    }

    protected virtual void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (Time.time >= lastAttackTime + attackCooldown)
            {
                if (collision.gameObject.TryGetComponent(out HealthComponent playerHealth))
                {
                    playerHealth.TakeDamage(damage);
                    Debug.Log("J'ai touché le joueur !");
                }

                ApplySelfKnockback(collision.transform.position);

                lastAttackTime = Time.time;
            }
        }
    }

    protected void ApplySelfKnockback(Vector3 playerPosition)
    {
        isRecoiling = true;

        Vector2 direction = (transform.position - playerPosition).normalized;

        rb.linearVelocity = Vector2.zero; 

        rb.AddForce(direction * recoilForce, ForceMode2D.Impulse);
        StartCoroutine(RecoverFromRecoil());
    }

    private IEnumerator RecoverFromRecoil()
    {
        yield return new WaitForSeconds(recoilDuration);

        isRecoiling = false;
    }
    public abstract void Attack();
}
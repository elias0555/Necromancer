using UnityEngine;

public class MeleeAbility : MonoBehaviour, ICombatAbility
{
    [SerializeField] private float damage = 10f;
    [SerializeField] private float range = 1.5f;
    [SerializeField] private float cooldown = 1f;
    [SerializeField] private float knockbackForce = 5f;

    public void ExecuteAttack(Vector2 origin, Vector2 direction, LayerMask targetLayer)
    {
        // Logique similaire à ton ancien BaseEnemy
        Vector2 hitPoint = origin + direction * (range * 0.5f);
        Collider2D[] hits = Physics2D.OverlapCircleAll(hitPoint, range, targetLayer);

        foreach (var hit in hits)
        {
            if (hit.TryGetComponent(out HealthComponent health))
            {
                health.TakeDamage(damage);
                // Optionnel : Ajouter du recul (Knockback) ici
            }
        }
    }

    public float GetRange() => range;
    public float GetCooldown() => cooldown;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(HealthComponent))]
public class UnitController : MonoBehaviour
{
    [Header("État Dynamique")]
    public float currentMoveSpeed;

    private Rigidbody2D _rb;
    private HealthComponent _health;
    private ICombatAbility _combatAbility;
    private SpriteRenderer _spriteRenderer;

    private bool _canAttack = true;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _health = GetComponent<HealthComponent>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Initialize(UnitProfileSO profile)
    {
        if (profile.visualSprite != null) _spriteRenderer.sprite = profile.visualSprite;
        currentMoveSpeed = profile.moveSpeed;
        _health.SetMaxHealth(profile.maxHealth); 

        foreach (Transform child in transform)
        {
            if (child.GetComponent<ICombatAbility>() != null) Destroy(child.gameObject);
        }

        if (profile.abilityPrefab != null)
        {
            GameObject abilityObj = Instantiate(profile.abilityPrefab, transform);
            abilityObj.transform.localPosition = Vector3.zero;
            _combatAbility = abilityObj.GetComponent<ICombatAbility>();
        }
    }

    public void Move(Vector2 direction)
    {
        _rb.linearVelocity = direction * currentMoveSpeed;

        // Flip du sprite basique
        if (direction.x > 0.1f) transform.localScale = new Vector3(-1, 1, 1);
        else if (direction.x < -0.1f) transform.localScale = new Vector3(1, 1, 1);
    }

    public void TryAttack(Vector2 targetPos, LayerMask targetLayer)
    {
        if (!_canAttack || _combatAbility == null) return;

        Vector2 direction = (targetPos - (Vector2)transform.position).normalized;

        _combatAbility.ExecuteAttack(transform.position, direction, targetLayer);

        StartCoroutine(CooldownRoutine());
    }

    public float GetAttackRange() => _combatAbility != null ? _combatAbility.GetRange() : 0.5f;

    private IEnumerator CooldownRoutine()
    {
        _canAttack = false;
        yield return new WaitForSeconds(_combatAbility.GetCooldown());
        _canAttack = true;
    }
}
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem; // Nécessaire pour Pointer.current

public class Player : MonoBehaviour
{
    [Header("Combat Settings")]
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private int attackDamage = 10;
    [SerializeField] private LayerMask enemyLayer;

    [Header("Visuals")]
    [SerializeField] private Color gizmoColor = Color.red;

    private Vector3 _lastHitPoint;
    private bool _showGizmo = false;

    public HealthComponent HealthComponent;

    void Update()
    {
        HandleInput();
    }

    private void HandleInput()
    {
        if (PlayerInputManager.Instance != null &&
            PlayerInputManager.Instance.playerControls.Player.Attack.WasPressedThisFrame())
        {
            PerformAttack();
        }
        else if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            PerformAttack();
        }
    }

    private void PerformAttack()
    {
        Vector2 mousePosition = Pointer.current.position.ReadValue();
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
        worldPosition.z = 0;

        _lastHitPoint = worldPosition;
        StartCoroutine(ShowGizmoTimer());


        Collider2D[] hits = Physics2D.OverlapCircleAll(worldPosition, attackRange);

        foreach (Collider2D hit in hits)
        {
            if (hit.gameObject == gameObject) continue;

            HealthComponent targetHealth = hit.GetComponentInParent<HealthComponent>();

            if (targetHealth != null)
            {
                targetHealth.TakeDamage(attackDamage);
                Debug.Log($"TOUCHÉ : {hit.name}");
            }
            else
            {
                Debug.Log($"Objet détecté : {hit.name}, mais PAS de HealthComponent trouvé dessus.");
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (_showGizmo)
        {
            Gizmos.color = gizmoColor;
            Gizmos.DrawWireSphere(_lastHitPoint, attackRange);

            Gizmos.color = new Color(gizmoColor.r, gizmoColor.g, gizmoColor.b, 0.3f);
            Gizmos.DrawSphere(_lastHitPoint, attackRange);
        }
    }

    private IEnumerator ShowGizmoTimer()
    {
        _showGizmo = true;
        yield return new WaitForSeconds(0.2f);
        _showGizmo = false;
    }
}
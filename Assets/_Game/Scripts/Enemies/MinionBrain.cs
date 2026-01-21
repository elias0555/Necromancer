using UnityEngine;

public class MinionBrain : UnitBrain
{
    private Transform _summoner;
    private Transform _currentTarget;

    [Header("Comportement")]
    [SerializeField] private float leashDistance = 8f; // Distance Max
    [SerializeField] private float detectionRadius = 6f; // Vision

    public void Initialize(Transform summoner)
    {
        _summoner = summoner;
    }

    protected override void Think()
    {
        if (_summoner == null) return;

        float distToSummoner = Vector2.Distance(transform.position, _summoner.position);

        // 1. PRIORITÉ : LA LAISSE (GDD 3.2)
        if (distToSummoner > leashDistance)
        {
            _currentTarget = null;
            MoveTowards(_summoner.position);
            return;
        }

        // 2. Acquisition de cible
        if (_currentTarget == null)
        {
            FindTarget();
            // Si pas de cible, on erre autour du joueur ou on le suit
            if (distToSummoner > 2f) MoveTowards(_summoner.position);
            else unit.Move(Vector2.zero);
        }
        else
        {
            // 3. Combat
            if (!_currentTarget.gameObject.activeInHierarchy) { _currentTarget = null; return; }

            float distToTarget = Vector2.Distance(transform.position, _currentTarget.position);

            if (distToTarget <= unit.GetAttackRange())
            {
                unit.Move(Vector2.zero);
                unit.TryAttack(_currentTarget.position, LayerMask.GetMask("Enemy"));
            }
            else
            {
                MoveTowards(_currentTarget.position);
            }
        }
    }

    private void MoveTowards(Vector2 target)
    {
        Vector2 dir = (target - (Vector2)transform.position).normalized;
        unit.Move(dir);
    }

    private void FindTarget()
    {
        // Cherche l'ennemi le plus proche
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, detectionRadius, LayerMask.GetMask("Enemy"));
        float closestDist = Mathf.Infinity;
        Transform bestTarget = null;

        foreach (var hit in hits)
        {
            float d = Vector2.Distance(transform.position, hit.transform.position);
            if (d < closestDist)
            {
                closestDist = d;
                bestTarget = hit.transform;
            }
        }
        _currentTarget = bestTarget;
    }
}
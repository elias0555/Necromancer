using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem; // Nécessaire pour Pointer.current

public class Player : MonoBehaviour, IEnemyTargetable
{
	private static Player instance;
	public static Player Instance => instance; 
	
    [Header("Combat Settings")]
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private int attackDamage = 10;
    [SerializeField] private LayerMask enemyLayer;

    [Header("Visuals")]
    [SerializeField] private Color gizmoColor = Color.red;

    private Vector3 _lastHitPoint;
    private bool _showGizmo = false;

	void Awake()
	{
		instance = this;
	}

	void Update()
    {
		
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

	public Vector3 Position() {
		return transform.position;
	}

    private IEnumerator ShowGizmoTimer()
    {
        _showGizmo = true;
        yield return new WaitForSeconds(0.2f);
        _showGizmo = false;
    }
}
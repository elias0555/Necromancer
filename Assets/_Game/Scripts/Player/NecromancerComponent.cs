using UnityEngine;
using UnityEngine.InputSystem; // For new Input System

public class NecromancerComponent : MonoBehaviour
{
    [Header("Necromancy Settings")]
    [SerializeField] private float detectionRadius = 5f; // Minion wander area
    [SerializeField] private float resurrectionRange = 3f; // Max distance to revive corpse
    [SerializeField] private LayerMask corpseLayer; // LayerMask set to "Corpse"
    [SerializeField] private int maxMinions = 10;

    private int currentMinionCount = 0;

    void Update()
    {
        HandleResurrectionInput();
    }

    private void HandleResurrectionInput()
    {
        if (PlayerInputManager.Instance.playerControls.Player.Necromance.WasPerformedThisFrame())
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero, 0.1f, corpseLayer);

            if (hit.collider != null)
            {
                float dist = Vector2.Distance(transform.position, hit.transform.position);

                if (dist <= resurrectionRange)
                {
                    if (hit.transform.TryGetComponent(out Corpse corpseScript))
                    {
                        Revive(corpseScript);
                        print("Resurrected a minion!");
                    }
                }
                else
                {
                    Debug.Log("Too far to resurrect!");
                }
            }
        }
    }

    private void Revive(Corpse corpse)
    {
        if (currentMinionCount >= maxMinions)
        {
            Debug.Log("Max minions reached!");
            return;
        }

        GameObject prefabToSpawn = corpse.GetMinionPrefab();

        if (prefabToSpawn != null)
        {
            GameObject newMinion = Instantiate(prefabToSpawn, corpse.transform.position, Quaternion.identity);
            MinionAI ai = newMinion.GetComponent<MinionAI>();
            if (ai != null)
            {
                ai.Initialize(transform, detectionRadius);
            }

            currentMinionCount++;

            Destroy(corpse.gameObject);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0, 1, 0, 0.3f);
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, resurrectionRange);
    }
}
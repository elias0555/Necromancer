using UnityEngine;
using UnityEngine.InputSystem;

public class NecromancerComponent : MonoBehaviour
{
    [Header("Paramètres")]
    [SerializeField] private GameObject genericMinionPrefab; // Un prefab VIDE avec juste UnitController + MinionBrain + Health
    [SerializeField] private float resurrectionRange = 4f;
    [SerializeField] private LayerMask corpseLayer;
    [SerializeField] private int maxMinions = 10;

    private int currentMinionCount = 0;

    void Update()
    {
        if (PlayerInputManager.Instance.playerControls.Player.Necromance.WasPerformedThisFrame())
        {
            AttemptNecromancy();
        }
    }

    private void AttemptNecromancy()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());

        // On cherche un cadavre sous la souris
        Collider2D hit = Physics2D.OverlapPoint(mousePos, corpseLayer);

        if (hit != null && hit.TryGetComponent(out Corpse corpse))
        {
            float dist = Vector2.Distance(transform.position, corpse.transform.position);
            if (dist <= resurrectionRange)
            {
                // Gestion Armée pleine vs non pleine (GDD)
                if (currentMinionCount < maxMinions)
                {
                    SpawnMinion(corpse);
                }
                else
                {
                    RecycleCorpse(corpse);
                }
            }
        }
    }

    private void SpawnMinion(Corpse corpse)
    {
        // 1. Récupérer les données
        UnitProfileSO profile = corpse.Consume();
        if (profile == null) return;

        // 2. Instancier le conteneur vide
        GameObject minionObj = Instantiate(genericMinionPrefab, corpse.transform.position, Quaternion.identity);

        // 3. Injecter les données (C'est là que l'héritage opère !)
        UnitController controller = minionObj.GetComponent<UnitController>();
        controller.Initialize(profile);

        // 4. Initialiser le cerveau
        minionObj.GetComponent<MinionBrain>().Initialize(transform);

        currentMinionCount++;
        Debug.Log($"Squelette {profile.unitName} levé !");
    }

    private void RecycleCorpse(Corpse corpse)
    {
        corpse.Consume();
        // Ajouter du Mana ici
        Debug.Log("Cadavre recyclé en Mana !");
    }
}
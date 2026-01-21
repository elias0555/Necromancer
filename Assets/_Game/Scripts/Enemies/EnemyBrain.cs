using UnityEngine;

public class EnemyBrain : UnitBrain
{
    private Transform player;
    [SerializeField] private UnitProfileSO myProfile; // Profil par défaut (pour les tests)

    protected override void Awake()
    {
        base.Awake();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        // Si un profil est déjà assigné dans l'éditeur, on s'initialise tout de suite
        if (myProfile != null)
        {
            unit.Initialize(myProfile);
        }
    }

    // --- C'est cette méthode qui manquait pour le WaveManager ---
    public void SetProfile(UnitProfileSO profile)
    {
        myProfile = profile;
        // On force l'initialisation du corps (UnitController) avec les nouvelles stats
        if (unit != null)
        {
            unit.Initialize(profile);
        }
    }
    // ------------------------------------------------------------

    public UnitProfileSO GetProfile() => myProfile; // Appelé par HealthComponent pour le cadavre

    protected override void Think()
    {
        if (player == null) return;

        float dist = Vector2.Distance(transform.position, player.position);

        // Si on est à portée d'attaque
        if (dist <= unit.GetAttackRange())
        {
            unit.Move(Vector2.zero); // On s'arrête
            // On attaque le Joueur ou un Minion
            unit.TryAttack(player.position, LayerMask.GetMask("Player", "Minion"));
        }
        else
        {
            // Sinon on poursuit le joueur
            Vector2 dir = (player.position - transform.position).normalized;
            unit.Move(dir);
        }
    }
}
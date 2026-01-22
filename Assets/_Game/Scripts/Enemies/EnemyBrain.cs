using UnityEngine;

public class EnemyBrain : UnitBrain
{
    private Transform player;
    [SerializeField] private UnitProfileSO myProfile; 

    protected override void Awake()
    {
        base.Awake();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (myProfile != null)
        {
            unit.Initialize(myProfile);
        }
    }

    public void SetProfile(UnitProfileSO profile)
    {
        myProfile = profile;
        if (unit != null)
        {
            unit.Initialize(profile);
        }
    }

    public UnitProfileSO GetProfile() => myProfile; 

    protected override void Think()
    {
        if (player == null) return;

        float dist = Vector2.Distance(transform.position, player.position);

        if (dist <= unit.GetAttackRange())
        {
            unit.Move(Vector2.zero); 
            unit.TryAttack(player.position, LayerMask.GetMask("Player", "Minion"));
        }
        else
        {
            Vector2 dir = (player.position - transform.position).normalized;
            unit.Move(dir);
        }
    }
}
using UnityEngine;

public interface ICombatAbility
{
    void ExecuteAttack(Vector2 origin, Vector2 direction, LayerMask targetLayer);

    float GetRange();
    float GetCooldown();
}
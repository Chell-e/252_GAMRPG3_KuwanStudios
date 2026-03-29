using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Status Effects/Vulnerable")]

public class StatusEffect_Vulnerable : BaseStatusEffect
{
    public float newDamageMultiplier;

    public override void OnApply(BaseEnemy _enemy, float _power)
    {
        // set enemy incoming damage
        _enemy.SetIncomingDamageModifier(newDamageMultiplier);
        Debug.Log("APPLIED HOLY WATER");
    }

    public override void OnTick(BaseEnemy _enemy, float _power, float _timeElapsed)
    {
        Debug.Log("TICKING HOLY WATER");
    }
    public override void OnExpire(BaseEnemy _enemy)
    {
        _enemy.SetIncomingDamageModifier(1.0f); // just resets to 1 flat for now

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffect_Vulnerable : BaseStatusEffect
{
    private float damageMultiplier;


    public override void OnApply(BaseEnemy _enemy)
    {
        damageMultiplier = potency;

        _enemy.SetIncomingDamageMultiplier(damageMultiplier);
        Debug.Log("Vulnerabled " + _enemy);
    }

    public override void OnUpdate(BaseEnemy _enemy, float _timeElapsed) { }
    public override void OnExpire(BaseEnemy _enemy)
    {
        _enemy.SetIncomingDamageMultiplier(1.0f);
    }
}

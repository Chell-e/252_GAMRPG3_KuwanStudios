using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StatusEffectInstance
{
    public BaseStatusEffect statusEffect;
    public float remainingTime;
    public float power;

    public StatusEffectInstance(BaseStatusEffect _statusEffectData, float _power)
    {
        statusEffect = _statusEffectData;
        remainingTime = statusEffect.lifetime;
        power = _power;
    }
}
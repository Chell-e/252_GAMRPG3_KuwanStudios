using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffectHandlerComponent : MonoBehaviour
{
    private BaseEnemy enemy;
    private List<StatusEffectInstance> activeEffects = new List<StatusEffectInstance>();


    private void Awake()
    {
        enemy = GetComponent<BaseEnemy>();
    }


    public void ApplyEffect(BaseStatusEffect _statusEffect, float _power)
    {
        var existingEffect = activeEffects.Find(effectInstance => effectInstance.statusEffect == _statusEffect); // lambda expression https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/operators/lambda-expressions

        // if effect already exists, just refresh
        if (existingEffect != null)
        {
            existingEffect.remainingTime = _statusEffect.lifetime;
            return;
        }

        // if not, make a new StatusEffectInstance
        StatusEffectInstance newEffectInstance = new StatusEffectInstance(_statusEffect, _power);
        activeEffects.Add(newEffectInstance);
        _statusEffect.OnApply(enemy, _power);

    }

    // Update is called once per frame
    void Update()
    {
        // Loop backwards so we can remove items safely
        for (int i = activeEffects.Count - 1; i >= 0; i--)
        {
            var effectInstance = activeEffects[i];
            effectInstance.remainingTime -= Time.deltaTime;

            effectInstance.statusEffect.OnTick(enemy, effectInstance.power, Time.deltaTime);

            if (effectInstance.remainingTime <= 0)
            {
                effectInstance.statusEffect.OnExpire(enemy);
                activeEffects.RemoveAt(i);
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class BaseStatusEffect
{
    protected float duration; // how long effect lasts
    protected float potency; // potency 

    protected float lifetime; //  runtime timer for duration

    public void Initialize(float _duration, float _potency)
    {
        this.duration = _duration;
        this.potency = _potency;

        this.lifetime = _duration;
    }

    public void Refresh()
    {
        lifetime = duration;
    }

    public void TickEffect(BaseEnemy _enemy, float _timeElapsed)
    {
        lifetime -= Time.deltaTime; // tick down lifetime, and call other functions
        OnUpdate(_enemy, _timeElapsed);
    }

    public bool IsFinished()
    {
        return lifetime <= 0;
    }

    public abstract void OnApply(BaseEnemy _enemy);
    public abstract void OnUpdate(BaseEnemy _enemy, float _timeElapsed);
    public abstract void OnExpire(BaseEnemy _enemy);
}

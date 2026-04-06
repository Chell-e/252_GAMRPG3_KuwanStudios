using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseWeapon : MonoBehaviour , ITooltipProvider
{
    [SerializeField] protected PlayerController playerController;
    [SerializeField] protected PlayerStats playerStats;
    [SerializeField] protected PlayerEvents playerEvents;
    public virtual void Initialize(PlayerController _playerController)
    {
        this.playerController = _playerController;
        this.playerStats = _playerController.playerStats;
        this.playerEvents = _playerController.Events;

        Subscribe();
    }

    protected virtual void Subscribe() { }
    protected virtual void Unsubscribe() { }

    protected virtual void OnDestroy()
    {
        Unsubscribe();
    }

    public virtual string GetTooltipText()
    {
        return "No override";
    }

    // new
    public virtual string GetName()
    {
        return "No name";
    }

    public virtual string GetDescription()
    {
        return "No description";
    }

    public virtual float GetFillProgress()
    {
        // If applicable, return the cooldown
        return 1.0f;
    }
}

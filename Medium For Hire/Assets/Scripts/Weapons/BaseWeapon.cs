using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseWeapon : MonoBehaviour
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
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEvents : MonoBehaviour
{
    // *****These events work for superstitions too

    public System.Action<DamageContext> OnBeforeGetHit;
    public System.Action<DamageContext> OnAfterGetHit;

    public System.Action<DamageContext> OnBeforeDealDamage;
    public System.Action<DamageContext> OnAfterDealDamage;

    public System.Action<DamageContext> OnAfterKillEnemy;

    public System.Action OnAimActivate;
    public System.Action OnAimDeactivate;
    public System.Action OnAimToggle;

    public System.Func<float, float> OnBeforeGetExp;
    public System.Func<float, float> OnAfterGetExp;

    public System.Action OnAfterGetUpgrade;

    public System.Action OnPlayerDeath;

    // *****Below is mostly for tutorial inputs
        // movement
    public System.Action OnPressW;
    public System.Action OnPressA;
    public System.Action OnPressS;
    public System.Action OnPressD;

    public System.Action OnAfterDash;

    //      # aiming & attacking
    // OnAimActivate, OnAimDeactivate, OnAfterDealDamage

    //      # upgrades & evolution 
    // OnAfterGetUpgrade
    // *OnAfterEvolution?

    //      # shrines
}

public class DamageContext
{
    public float damage;
    
        // taking damage
    public bool isNulled = false;

        // dealing damage
    public BaseEnemy target;
}

public class ExpContext
{
    public float exp;
}
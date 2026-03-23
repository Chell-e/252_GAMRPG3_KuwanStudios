using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEvents : MonoBehaviour
{
    public System.Action<DamageContext> OnBeforeGetHit;
    public System.Action<DamageContext> OnAfterGetHit;

    public System.Action<DamageContext> OnBeforeDealDamage;
    public System.Action<DamageContext> OnAfterDealDamage;

    public System.Action OnAimActivate;
    public System.Action OnAimDeactivate;
    public System.Action OnAimToggle;

    public System.Func<float, float> OnBeforeGetExp;
    public System.Func<float, float> OnAfterGetExp;


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
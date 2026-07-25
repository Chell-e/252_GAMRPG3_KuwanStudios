using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Status Effects/Buntot Pagi/Reeled")]
public class StatusEffect_Reeled : BaseStatusEffect
{
    public float pullPower;

    public float potencyModifier;

    public float stoppingDistance = 0.5f;

    public override void OnApply(BaseEnemy _enemy, float _potencyModifier)
    {
        // just power = potency multiplier ig?
        //Debug.Log("APPLIED REELED!");
        potencyModifier = _potencyModifier;
    }

    public override void OnTick(BaseEnemy _enemy, float _potencyModifier, float _timeElapsed)
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 mouseWorldPos = new Vector3(mousePos.x, mousePos.y, 0);

        float finalPullPower = pullPower * _potencyModifier; ;

        float dist = Vector2.Distance(_enemy.transform.position, mouseWorldPos);
        if (dist > stoppingDistance)
        {
            Vector2 pullForce = (mouseWorldPos - _enemy.transform.position).normalized * finalPullPower;
            _enemy.GetComponent<Rigidbody2D>().AddForce(pullForce);
        }
        //Debug.Log("TICKED REELED!");

    }
    public override void OnExpire(BaseEnemy _enemy)
    {
        //Debug.Log("EXPIRED REELED!");
    }
}

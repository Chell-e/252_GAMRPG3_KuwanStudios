using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathVFX  : MonoBehaviour
{
    public void Despawn()
    {
        PoolManager.ReturnObjectToPool(gameObject);
    }
}

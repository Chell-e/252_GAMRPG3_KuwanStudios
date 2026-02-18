using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EliteTiyanakTrigger : MonoBehaviour
{
    public EliteTiyanakAI tiyanakAI;
    public bool hasEnteredLureZone;
    bool hasEnteredTransformZone;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var player = collision.gameObject.GetComponent<PlayerController>();

        if (player == null)
            return;
        
        if (gameObject.name == "LureZone" && !hasEnteredLureZone)
        {
            //hasEnteredLureZone = true;
            if (tiyanakAI.currentState == EliteTiyanakState.Approach)
            {
                 tiyanakAI.ChangeState(EliteTiyanakState.Lure);
            }
        }
        else if (gameObject.name == "TransformZone" && !hasEnteredTransformZone)
         {
             //hasEnteredTransformZone = true;
             if (tiyanakAI.currentState == EliteTiyanakState.Lure)
             {
                 tiyanakAI.ChangeState(EliteTiyanakState.Transform);
             }
         }
        
    }
}

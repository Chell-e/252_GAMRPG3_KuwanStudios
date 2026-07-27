using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class TriggerZone : MonoBehaviour
{
    public bool oneShot = false;
    public bool alreadyEntered = false;
    public bool alreadyExited = false;

    public UnityEvent onTriggerEnter;
    public UnityEvent onTriggerExit;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (alreadyEntered) return;
        if (!collision.GetComponent<PlayerController>()) return;

        onTriggerEnter?.Invoke();

        if (oneShot) alreadyEntered = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (alreadyExited) return;
        if (!collision.GetComponent<PlayerController>()) return;

        onTriggerExit?.Invoke();    

        if (oneShot) alreadyExited = true;
    }
}

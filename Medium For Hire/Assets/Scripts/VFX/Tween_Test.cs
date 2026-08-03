using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tween_Test : MonoBehaviour
{
    [Header("SETTINGS")]
    public float tweenDuration;

    public void OnClose()
    {
        LeanTween.scale(gameObject, new Vector3(0, 0, 0), 0.5f);
    }
}

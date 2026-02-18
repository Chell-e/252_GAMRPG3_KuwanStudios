using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class HitStop : MonoBehaviour
{
    public static HitStop Instance;
    private Coroutine currentCoroutine;

    [SerializeField] private float stopDuration = 0.1f;    

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
        DontDestroyOnLoad(gameObject);
    }

    public void Stop(float duration = -1f)
    {
        if (duration < 0f)
            duration = stopDuration;

        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine);

        currentCoroutine = StartCoroutine(Wait(duration));
    }

    IEnumerator Wait(float duration)
    {
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(duration);
       
        Time.timeScale = 1f;
        currentCoroutine = null;
    }
}

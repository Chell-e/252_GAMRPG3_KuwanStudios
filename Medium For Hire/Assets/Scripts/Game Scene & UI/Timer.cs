using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    [SerializeField] public PlayerEvents Events;

    [SerializeField] TextMeshProUGUI timerText;
    private float elapseTime;

    public bool isTimerRunning = true;

    private void OnEnable()
    {
        Events.OnPlayerDeath += StopTimer;
    }

    private void Update()
    {
        if (!isTimerRunning)
            return;

        elapseTime += Time.deltaTime;
        timerText.text = elapseTime.ToString();

        int minutes = Mathf.FloorToInt(elapseTime / 60);
        int seconds = Mathf.FloorToInt(elapseTime % 60);

        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    private void StopTimer()
    {
        isTimerRunning = false;
    }

    private void OnDisable()
    {
        Events.OnPlayerDeath -= StopTimer;
    }
}

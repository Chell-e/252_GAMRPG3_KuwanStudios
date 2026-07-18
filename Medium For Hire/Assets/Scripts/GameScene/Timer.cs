using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    [SerializeField] public PlayerEvents Events;

    [SerializeField] TextMeshProUGUI timerText;
    public float elapseTime;
    public bool isTimerRunning = true;

    [Header("BOSS TIMER")]
    public float bossTimer = 15.0f;

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

        // for boss timer
        float targetBossSeconds = bossTimer * 60f;
        if (UIManager.Instance != null && UIManager.Instance.BossTimerForeground != null)
        {
            float progressPercentage = Mathf.Clamp01(elapseTime / targetBossSeconds);
            UIManager.Instance.BossTimerForeground.fillAmount = progressPercentage;
        }
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

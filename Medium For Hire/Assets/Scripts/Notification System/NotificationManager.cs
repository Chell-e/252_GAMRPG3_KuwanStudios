using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;

public class NotificationManager : MonoBehaviour
{
    public static NotificationManager Instance;

    [Header("Notification UI")]
    [SerializeField] private GameObject notificationPanel;
    [SerializeField] private TextMeshProUGUI notificationText;

    private CanvasGroup notificationCanvasGroup;
    private Queue<NotificationSO> notificationQueue = new Queue<NotificationSO>();
    private bool isDisplayingNotification = false;

    private void Awake()
    {
        // singleton 
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        notificationCanvasGroup = notificationPanel.GetComponent<CanvasGroup>();
        notificationCanvasGroup.alpha = 0;
    }

    public void ShowNotification(NotificationSO notificationData)
    {
        notificationQueue.Enqueue(notificationData);
        if (!isDisplayingNotification)
        {
            StartCoroutine(DisplayNotification());
        }
    }

    private IEnumerator DisplayNotification()
    {
        isDisplayingNotification = true;
        while (notificationQueue.Count > 0)
        {
            NotificationSO data = notificationQueue.Dequeue();

            // apply SO data
            notificationText.text = data.Message;

            // fade in
            yield return StartCoroutine(FadeCanvasGroup(notificationCanvasGroup, true, data.FadeDuration));

            // display
            yield return new WaitForSeconds(data.DisplayDuration);

            // fade out
            yield return StartCoroutine(FadeCanvasGroup(notificationCanvasGroup, false, data.FadeDuration));
        }
        
        isDisplayingNotification= false;
    }

    public IEnumerator FadeCanvasGroup(CanvasGroup canvasGroup, bool fadeIn, float duration)
    {
        float targetAlpha = fadeIn ? 1.0f : 0.0f;
        float initialAlpha = canvasGroup.alpha;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;

            canvasGroup.alpha = Mathf.Lerp(initialAlpha, targetAlpha, elapsedTime/duration);
            yield return null;
        }
        canvasGroup.alpha = targetAlpha;
    }
}

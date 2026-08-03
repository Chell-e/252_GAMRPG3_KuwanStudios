using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VFX_TakingDMG : MonoBehaviour
{
    [SerializeField] private Image vfxImg;
    [SerializeField] private float fadeSpeed = 2f;
    [SerializeField] private float maxAlpha = 0.6f;

    private Coroutine fadeCoroutine;

    private void Awake()
    {
        vfxImg = GetComponent<Image>();

        Color c = vfxImg.color;
        c.a = 0f;
        vfxImg.color = c;
    }

    private void Start()
    {
        if (PlayerController.Instance != null)
            PlayerController.Instance.Events.OnAfterGetHit += OnPlayerHit;
    }

    private void OnDisable()
    {
        if (PlayerController.Instance != null)
            PlayerController.Instance.Events.OnAfterGetHit -= OnPlayerHit;
    }

    private void OnPlayerHit(DamageContext context)
    {
        if (!context.isNulled) ShowVFX();
    }

    private void Update()
    {
        if (vfxImg.color.a > 0f)
        {
            Color c = vfxImg.color;
            c.a = Mathf.MoveTowards(c.a, 0f, fadeSpeed * Time.deltaTime);
            vfxImg.color = c;
        }
    }

    private void ShowVFX()
    {
        Color c = vfxImg.color;
        c.a = maxAlpha;
        vfxImg.color = c;
    }
}

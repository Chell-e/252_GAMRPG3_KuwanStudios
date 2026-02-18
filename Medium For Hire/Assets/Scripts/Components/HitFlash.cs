using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitFlash : MonoBehaviour
{
    [ColorUsage(true, true)]
    [SerializeField] private Color flashcolor = Color.white;
    [SerializeField] private float flashDuration = 0.25f;
    [SerializeField] private AnimationCurve flashSpeedCurve; // Customize the speed of the flash over time

    private SpriteRenderer spriteRenderer;
    private MaterialPropertyBlock mpb;
    private Coroutine hitFlashCoroutine;

    private void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        mpb = new MaterialPropertyBlock();
        ResetFlash();
    }

    public void TriggerHitFlash()
    {
        // If game object is inactive, don't start
        if (!gameObject.activeInHierarchy)
        {
            return;
        }

        // Stop any existing hit flash coroutine
        if (hitFlashCoroutine != null)
            StopCoroutine(hitFlashCoroutine);

        hitFlashCoroutine = StartCoroutine(HitFlasher());
    }

    private IEnumerator HitFlasher()
    {
        spriteRenderer.GetPropertyBlock(mpb);
        SetColor();
        SetFlashAmount();
        spriteRenderer.SetPropertyBlock(mpb);

        float currentFlashAmount = 0f;
        float elapsedTime = 0f;
        while (elapsedTime < flashDuration)
        {
            elapsedTime += Time.deltaTime;
            currentFlashAmount = Mathf.Lerp(1f, flashSpeedCurve.Evaluate(elapsedTime), (elapsedTime / flashDuration));
            
            spriteRenderer.GetPropertyBlock(mpb);
            mpb.SetFloat("_FlashAmount", currentFlashAmount);
            spriteRenderer.SetPropertyBlock(mpb);

            yield return null;
        }

        // Ensure flash amount is cleared at the end of the flash
        ClearFlashAmount();
        hitFlashCoroutine = null;
    }

    private void SetColor()
    {
        mpb.SetColor("_FlashColor", flashcolor);
    }

    private void SetFlashAmount()
    {
        mpb.SetFloat("_FlashAmount", 1f);
    }

    private void ClearFlashAmount()
    {
        spriteRenderer.GetPropertyBlock(mpb);
        mpb.SetFloat("_FlashAmount", 0f);
        spriteRenderer.SetPropertyBlock(mpb);
    }

    // Ensure flash is reset when object is disabled 
    private void ResetFlash()
    {
        if (hitFlashCoroutine == null) return;

        spriteRenderer.GetPropertyBlock(mpb);
        mpb.SetFloat("_FlashAmount", 0f);
        mpb.SetColor("_FlashColor", flashcolor);
        spriteRenderer.SetPropertyBlock(mpb);
    }
}

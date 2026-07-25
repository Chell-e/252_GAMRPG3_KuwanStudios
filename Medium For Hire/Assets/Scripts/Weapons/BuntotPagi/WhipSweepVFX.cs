using UnityEngine;

public class WhipSweepVFX : MonoBehaviour
{
    [Header("SETTINGS")]
    public float duration = 0.25f;
    public AnimationCurve scaleCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public AnimationCurve alphaCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);


    [Header("DEBUG")]
    private SpriteRenderer sr;

    private float timer;
    private float maxScale;

    public void Init(float radius)
    {
        sr = GetComponent<SpriteRenderer>();

        timer = 0f;

        // Diameter because sprite scale is full size
        maxScale = radius;

        transform.localScale = Vector3.zero;

        /*if (sr != null)
            sr.color = color;*/
    }



    void Update()
    {
        timer += Time.deltaTime;

        float t = timer / duration;

        if (t >= 1f)
        {
            Destroy(gameObject);
            return;
        }

        // Scale
        float scale = scaleCurve.Evaluate(t) * maxScale;
        transform.localScale = new Vector3(scale, scale, 1f);

        float spin = scaleCurve.Evaluate(t) * maxScale;
        transform.localRotation = Quaternion.Euler(0f, 0f, -spin * 100f);

        // Fade
        if (sr != null)
        {
            Color c = sr.color;
            c.a = alphaCurve.Evaluate(t);
            sr.color = c;
        }
    }
}
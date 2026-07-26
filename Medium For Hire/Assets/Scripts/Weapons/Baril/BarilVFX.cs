using UnityEngine;

public class BarilVFX : MonoBehaviour
{
    [Header("Animation")]
    public float duration = 0.1f;
    public AnimationCurve scaleCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public AnimationCurve alphaCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);

    [Header("REFERENCES")]
    [SerializeField] public SpriteRenderer sr;

    private float timer;
    private float maxScale;

    public void Init(float radius)
    {
        if (sr == null)
            sr = GetComponent<SpriteRenderer>();

        timer = 0f;

        // Diameter because sprite scale is full size
        maxScale = radius;

        //transform.localScale = Vector3.zero;

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
        transform.localScale = new Vector3(scale, scale*1.5f, 1f);

        // Fade
        if (sr != null)
        {
            Color c = sr.color;
            c.a = alphaCurve.Evaluate(t);
            sr.color = c;
        }
    }
}
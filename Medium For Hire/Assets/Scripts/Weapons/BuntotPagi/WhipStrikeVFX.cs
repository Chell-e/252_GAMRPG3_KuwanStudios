using UnityEngine;

public class WhipStrikeVFX : MonoBehaviour
{
    [Header("SETTINGS")]
    public float duration = 0.25f;
    public AnimationCurve scaleCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public AnimationCurve alphaCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);


    [Header("DEBUG")]
    private SpriteRenderer sr;
    [SerializeField] private Animator anim;
    //[SerializeField] private Sprite[] spritesheet; 

    private float timer;

    public void Init(Vector2 _size, Vector2 _localPosition)
    {
        sr = GetComponent<SpriteRenderer>();

        //int randomIndex = Random.Range(0, spritesheet.Length);
        //sr.sprite = spritesheet[randomIndex];


        sr.size = _size;
        transform.position = (Vector2)transform.position + _localPosition;
        if (_localPosition.x < 0)
            transform.localScale = new Vector2(-1, 1);

        timer = 0f;

        // Diameter because sprite scale is full size

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

        // Fade
        if (sr != null)
        {
            Color c = sr.color;
            c.a = alphaCurve.Evaluate(t);
            sr.color = c;
        }
    }
}
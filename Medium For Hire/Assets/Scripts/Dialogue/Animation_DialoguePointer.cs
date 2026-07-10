using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animation_DialoguePointer : MonoBehaviour
{
    public enum DialoguePointerState
    {
        Still, // when the dialogue is being type-written
        Bobbing, // when the dialogue is complete
        Hidden // when RMB to "hide" dialogue box
    }

    [Header("SETTINGS")]
    public float bobSpeed = 5f;
    public float bobAmount = 8f;

    [SerializeField] private DialoguePointerState currentState = DialoguePointerState.Still;

    private Vector3 startPosition;
    private Vector3 startScale;

    // Start is called before the first frame update
    void Start()
    {
        startPosition = transform.localPosition;
        startScale = transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            case DialoguePointerState.Still:
                ResetArrow();
                break;

            case DialoguePointerState.Bobbing:
                Bob(false);
                break;

            case DialoguePointerState.Hidden:
                Bob(true);
                break;
        }
    }

    void Bob(bool upsideDown)
    {
        float offset = Mathf.Sin(Time.unscaledTime * bobSpeed) * bobAmount;

        Debug.Log("Time.unscaledTime: " + Time.time);
        Debug.Log($"bobSpeed: {bobSpeed}; bobAmount: {bobAmount}");

        transform.localPosition = startPosition + (Vector3.up * Mathf.Abs(offset));

        Vector3 scale = startScale;
        scale.y = upsideDown ? -Mathf.Abs(scale.y) : Mathf.Abs(scale.y);
        transform.localScale = scale;
    }

    void ResetArrow()
    {
        transform.localPosition = startPosition;

        Vector3 scale = startScale;
        scale.y = Mathf.Abs(scale.y);
        transform.localScale = scale;
    }

    public void SetState(DialoguePointerState state)
    {
        currentState = state;
    }
}

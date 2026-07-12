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

        [Header("REFERENCES")]
    [SerializeField] private DialogueUI ui;


    //
    // =====================
        [Header("DEBUG")]
    [SerializeField] private DialoguePointerState currentState = DialoguePointerState.Still;

    private Vector3 startPosition;
    private Vector3 startScale;

    // Start is called before the first frame update
    void Start()
    {
        startPosition = transform.localPosition;
        startScale = transform.localScale;

        ui.OnDialogueTyping += OnDialogueTyping;
        ui.OnDialogueFinished += OnDialogueFinished;
        ui.OnDialogueHidden += OnDialogueHidden;
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            case DialoguePointerState.Still:
                ResetArrow();
                Debug.Log("POINTER DETECTS TYPING COMPLETE!");
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

    // ==========
    // EVENTS
    private void OnDialogueTyping()
    {
        SetState(DialoguePointerState.Still);
    }

    private void OnDialogueFinished()
    {
        SetState(DialoguePointerState.Bobbing);
    }
    private void OnDialogueHidden(bool isHidden)
    {
        if (isHidden)
        {
            SetState(DialoguePointerState.Hidden);
        }
        else
        {
            SetState(DialoguePointerState.Still);
        }

    }
}

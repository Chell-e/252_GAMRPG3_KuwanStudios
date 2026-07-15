using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
//using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.UI;
using static Animation_DialoguePointer;


public class DialogueUI : MonoBehaviour
{
        [Header("SETTINGS")]
    public int charPerSecond = 5;

        [Header("REFERENCES")]
    [SerializeField] public Image illustration;
    [SerializeField] public Image portrait;
    [SerializeField] public TMP_Text dialogueText;
    [SerializeField] public TMP_Text nameText;


    [SerializeField] private GameObject framePortrait;
    [SerializeField] private GameObject frameBody;
    [SerializeField] private GameObject frameName;
    
    [SerializeField] public Animation_DialoguePointer pointer;
    [SerializeField] public Canvas rootCanvas; // if assigned, moves the illustration to fill the screen automatically

    
    //============================
    //privates
    private Coroutine typingAnimation;
    public bool isTyping;
    public bool isHidden = false;

    // events
    public event Action OnDialogueTyping;
    public event Action OnDialogueFinished;
    public event Action<bool> OnDialogueHidden;
    //
    // ===========================

    private void Awake()
    {
        if (rootCanvas != null) // if rootCanvas is assigned, stretch the illustration to fit it
        {
            StretchIllustrationToCanvas(rootCanvas);
        }
    }

    public void StretchIllustrationToCanvas(Canvas canvas)
    {
        // this stretches the illustration to fit the canvas/screen
        illustration.rectTransform.SetParent(canvas.transform, false);

        RectTransform illustrationRect = illustration.GetComponent<RectTransform>();

        illustrationRect.anchorMin = Vector3.zero;
        illustrationRect.anchorMax = Vector3.one;
        illustrationRect.offsetMin = Vector2.zero;
        illustrationRect.offsetMax = Vector2.zero;
    }

    public void PlayDialogue(DialogueLine dialogueLine)
    {
        // this method assigns the DialogueLine we must display

        this.illustration.sprite = Resources.Load<Sprite>
            (
            "Illustrations/" + dialogueLine.illustration
            );

        this.portrait.sprite = Resources.Load<Sprite>
            (
            "Portraits/" + dialogueLine.portrait
            );

        this.dialogueText.text = dialogueLine.bodyText;
        if (!isHidden)
            typingAnimation = StartCoroutine(TypewriteDialogue());
        else
            SkipTyping();

            this.nameText.text = dialogueLine.nameText;
    }

    public void ToggleHide()
    {
        SkipTyping();
        isHidden = !isHidden;

        framePortrait.gameObject.SetActive(!isHidden);
        frameBody.gameObject.SetActive(!isHidden);
        frameName.gameObject.SetActive(!isHidden);

        OnDialogueHidden.Invoke(isHidden);
    }

    IEnumerator TypewriteDialogue()
    {
        OnDialogueTyping?.Invoke();
        Debug.Log("CURRENTLY TYPING!");
        isTyping = true;

        //dialogueText.text = message;
        dialogueText.ForceMeshUpdate();

        dialogueText.maxVisibleCharacters = 0;

        int total = dialogueText.textInfo.characterCount;

        for (int i = 0; i <= total; i++)
        {
            dialogueText.maxVisibleCharacters = i;
            yield return new WaitForSecondsRealtime(1f / charPerSecond);
        }

        Debug.Log("DONE TYPING!");
        isTyping = false;
        OnDialogueFinished?.Invoke();
    }

    public void SkipTyping()
    {
        if (!isTyping)
            return;

        StopCoroutine(typingAnimation);

        dialogueText.maxVisibleCharacters = dialogueText.textInfo.characterCount;
        isTyping = false;


        OnDialogueFinished?.Invoke();
    }

}

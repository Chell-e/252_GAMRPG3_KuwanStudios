using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.VersionControl;
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

    [SerializeField] public Animation_DialoguePointer pointer;

    private Coroutine typingAnimation;
    private bool isTyping;

    // events
    public event Action OnDialogueTyping;
    public event Action OnDialogueFinished;
    public event Action<bool> OnDialogueHidden;
    //

    public void SetDialogue(DialogueLine dialogueLine)
    {
        this.illustration.sprite = Resources.Load<Sprite>
            (
            "Illustrations/" + dialogueLine.illustration
            );

        this.portrait.sprite = Resources.Load<Sprite>
            (
            "Portraits/" + dialogueLine.portrait
            );

        Debug.Log("Loading portrait: " + dialogueLine.portrait);

        this.dialogueText.text = dialogueLine.bodyText;
        typingAnimation = StartCoroutine(TypewriteDialogue());


        this.nameText.text = dialogueLine.nameText;

    }


    IEnumerator TypewriteDialogue()
    {
        Debug.Log("CURRENTLY TYPING!");
        //isTyping = true;

        //dialogueText.text = message;
        dialogueText.ForceMeshUpdate();

        dialogueText.maxVisibleCharacters = 0;

        int total = dialogueText.textInfo.characterCount;

        for (int i = 0; i <= total; i++)
        {
            dialogueText.maxVisibleCharacters = i;
            yield return new WaitForSecondsRealtime(1f / charPerSecond);
        }

    }

    public void SkipTyping()
    {
        //if (!isTyping)
        //    return;

        StopCoroutine(typingAnimation);

        dialogueText.maxVisibleCharacters = dialogueText.textInfo.characterCount;
        //isTyping = false;
    }


    public void ClearText()
    {
        dialogueText.text = "";
    }

    public void SetPointerState(DialoguePointerState pointerState)
    {
        pointer.SetState(pointerState);
    }
}

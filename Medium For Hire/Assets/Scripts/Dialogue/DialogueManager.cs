using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;
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
    }
    // singleton stuff


    // enums
    public enum DialogueBoxState
    {
        Typing, // playing type-writer effect
        Finished, // done
        Hidden // boxes hidden
    }
    // enums


    [Header("SETTINGS")]


    [Header("DEBUG")]
    public DialogueBoxState state = DialogueBoxState.Typing;
    public DialogueFile currentSequence; // which dialogue to load 
    public int currentIndex; // current dialogue line displayed
    public int exploredIndex; // the "farthest" dialogue line displayed; caps the backtracking scroll function 

    [Header("REFERENCES")]
    public GameObject ui;
    public DialogueUI uiData; // the ui object to manipulate/load



    public void StartDialogue(DialogueFile dialogueSequence)
    {
        ui.SetActive(true);

        currentSequence = dialogueSequence;
        currentIndex = 0;
        exploredIndex = 0;

        PauseGame();
    }

    public void DisplayCurrentLine()
    {
        uiData.SetDialogue( currentSequence.dialogueLines[currentIndex] );
    }

    /*IEnumerator TypeText(string message)
    {

        
        *//*this.state = DialogueBoxState.Typing;

        dialogueText.text = message;
        dialogueText.ForceMeshUpdate();

        dialogueText.maxVisibleCharacters = 0;

        int total = dialogueText.textInfo.characterCount;

        for (int i = 0; i <= total; i++)
        {
            dialogueText.maxVisibleCharacters = i;
            yield return new WaitForSecondsRealtime(1f / charactersPerSecond);
        }

        isTyping = false;*//*
    }*/

    // ====================== DIALOGUE NAVIGATION
    public void NextLine()
    {
        if (currentIndex >= currentSequence.dialogueLines.Length-1)
        {
            EndDialogue();
            return;
        }

        currentIndex++;
        DisplayCurrentLine();

        if (currentIndex > exploredIndex)
            exploredIndex = currentIndex;
    }

    public void PreviousLine()
    {
        if (currentIndex == 0)
            return;

        currentIndex--;
        DisplayCurrentLine();
    }
    // ====================== DIALOGUE NAVIGATION

    private void Start()
    {

    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) //on LMB
        {
            if (GameStateManager.Instance == null
                || GameStateManager.Instance.currentState == GameState.Cutscene)
            {
                // bypass the game state check if GameStateManager does not exist!
                // ------

                NextLine();
            }

        }

        if (Input.GetMouseButtonDown(1)) //RMB
        {

            if (GameStateManager.Instance == null
                || GameStateManager.Instance.currentState == GameState.Cutscene)
            {
                // bypass the game state check if GameStateManager does not exist!
                // ------

                //ui.ToggleVisibility();
            }

        }

        float wheel = Input.mouseScrollDelta.y;

        if (wheel > 0)
        {
            if (GameStateManager.Instance == null
                || GameStateManager.Instance.currentState == GameState.Cutscene)
            {
                // bypass the game state check if GameStateManager does not exist!
                // ------

                PreviousLine();
            }
        }


        if (wheel < 0)
        {
            if (GameStateManager.Instance == null
                || GameStateManager.Instance.currentState == GameState.Cutscene)
            {
                // bypass the game state check if GameStateManager does not exist!
                // ------

                if (currentIndex < exploredIndex)
                    NextLine();
            }
        }
            
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
    }

    public void EndDialogue()
    {
        ui.SetActive(false);
        ResumeGame();
    }

    public void SetState(DialogueBoxState dialogueState)
    {
        this.state = dialogueState;
    }
}

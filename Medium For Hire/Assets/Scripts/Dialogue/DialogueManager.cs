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

    [Header("SETTINGS")]
        [Tooltip("- Whether on dialogue end, load to the target scene.")]
    public bool changeSceneOnEnd;
    public string targetScene;

    [Header("DEBUG")]
    public DialogueFile currentSequence; // which dialogue to load 
    public int currentIndex; // current dialogue line displayed
    public int exploredIndex; // the "farthest" dialogue line displayed; caps the backtracking scroll function 

    [Header("REFERENCES")]
    public DialogueUI dialogueUI; // the ui object to manipulate/load

    public System.Action OnDialogueEnd;

    public void StartDialogue(DialogueFile dialogueSequence)
    {
        dialogueUI.gameObject.SetActive(true);

        currentSequence = dialogueSequence;
        currentIndex = 0;
        exploredIndex = 0;

        PauseGame();
    }

    public void DisplayCurrentLine()
    {
        dialogueUI.PlayDialogue( currentSequence.dialogueLines[currentIndex] );
    }

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

    public void RunLeftClick()
    {
        // if dialogue is hidden, dont execute
        if (dialogueUI.isHidden == true)
            return;

        // if dialogue is playing, skip typing first
        if (dialogueUI.isTyping)
        {
            dialogueUI.SkipTyping();
            return;
        }

        NextLine();
    }

    public void RunRightClick()
    {
        dialogueUI.ToggleHide();
    }

    public void RunScrollUp()
    {
        dialogueUI.SkipTyping();
        PreviousLine();
        dialogueUI.SkipTyping();
    }

    public void RunScrollDown()
    {
        if (currentIndex < exploredIndex)
        {
            NextLine();
            dialogueUI.SkipTyping();
        }
    }
    // ====================== DIALOGUE NAVIGATION



    void Update()
    {
        if (GameStateManager.Instance == null
                || GameStateManager.Instance.currentState == GameState.Cutscene)
        {
            // bypass the game state check if GameStateManager does not exist!
            // ------

            if (Input.GetMouseButtonDown(0)) //on LMB
                RunLeftClick();

            if (Input.GetMouseButtonDown(1)) //on LMB
                RunRightClick();


            float wheel = Input.mouseScrollDelta.y;
            if (wheel > 0)
                RunScrollUp();

            if (wheel < 0)
                RunScrollDown();
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
        dialogueUI.gameObject.SetActive(false);
        ResumeGame();

        if (changeSceneOnEnd)
        {
            GameSceneManager.Instance.LoadScene(targetScene);
        }

        OnDialogueEnd?.Invoke();
    }
}

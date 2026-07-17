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
    [SerializeField] public List<GameState> actionableGameStates;
        [Tooltip("- Whether on dialogue end, load to the target scene.")]
    public bool loadSceneOnEnd;
    public string targetScene;

    [Header("DEBUG")]
    public GameState previousGameState; // STORE WHAT THE GameState was before initiating dialogue!!!!!!!
    public DialogueFile currentSequence; // which dialogue to load 
    public int currentIndex; // current dialogue line displayed
    public int exploredIndex; // the "farthest" dialogue line displayed; caps the backtracking scroll function 
    

    [Header("REFERENCES")]
    public DialogueUI dialogueUI; // the ui object to manipulate/load

    public System.Action OnDialogueEnd;



    // * DRIVER CODE
    // mainly Start() and Update()
    void Update()
    {
        if (CheckIsGameStateActionable() == false)
            return;


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
    // * DRIVER CODE


    // *** CORE LOGIC
    // these are functions that coordinate smaller functions below
    public void StartDialogue(DialogueFile dialogueSequence, GameState gameState = GameState.Cutscene)
    {
        dialogueUI.gameObject.SetActive(true);

        currentSequence = dialogueSequence;
        currentIndex = 0;
        exploredIndex = 0;

        previousGameState = GameStateManager.Instance.currentState;
        GameStateManager.Instance?.SetState(gameState);

        DisplayCurrentLine();
    }
    public void EndDialogue()
    {
        if (loadSceneOnEnd)
            GameSceneManager.Instance.LoadScene(targetScene);


        dialogueUI.gameObject.SetActive(false);
        GameStateManager.Instance?.SetState(previousGameState);


        // event
        OnDialogueEnd?.Invoke();
    }
    // *** CORE LOGIC


    // ** SUB FUNCTIONS
    // more "individual" functions
    public void DisplayCurrentLine()
    {
        dialogueUI.PlayDialogue(currentSequence.dialogueLines[currentIndex]);
    }
    
    public void NextLine()
    {
        if (currentIndex >= currentSequence.dialogueLines.Length - 1)
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
    // ** SUB FUNCTIONS


    // TOOLS
    // external, getters/setters, non-method stuff (e.g., IEnumerator)
    private bool CheckIsGameStateActionable()
    {
        // evaluates whether the current GameState matches actionableGameStates 

        if (GameStateManager.Instance == null)
        {
            // IF GameStateManager DOES NOT EXIST
            // LET US THROUGH!!!
            return true;
        }


        foreach (GameState actionableState in actionableGameStates)
        {
            // FOR EVERY LISTED GameState under actionableGameStates...
            if (GameStateManager.Instance.currentState == actionableState)
            {
                // IF CURRENT GAME STATE MATCHES ANY
                // LET US THROUGH!!!
                return true;
            }
        }

        // OR ELSE, BREAK!
        return false;
    }
    // TOOLS


    // EVENTS & LISTENERS
    // put events and listeners here

    // EVENTS & LISTENERS

}

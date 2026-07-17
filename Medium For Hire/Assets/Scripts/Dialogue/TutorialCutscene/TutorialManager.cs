using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    // singleton stuff
    public static TutorialManager Instance;
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

    // This is a HIGHLY SPECIFIC CLASS.
    // so it's probably OK to make stuff here HARD-CODED.


    [Header("DEBUG")]
    [SerializeField] private int tutorialIndex = 0;
    [SerializeField] private bool isDialogueActive = false;
    private Coroutine successWaitTime;

    [SerializeField] private bool hasMovedN = false;
    [SerializeField] private bool hasMovedS = false;
    [SerializeField] private bool hasMovedE = false;
    [SerializeField] private bool hasMovedW = false;

    [SerializeField] private bool hasAimActivated = false;
    [SerializeField] private bool hasAimDeactivated = false;


    [Header("REFERENCES")]
        // i stored the files in another class cuz its messy on the editor
    [SerializeField] TutorialFiles tutorialFiles;
    [SerializeField] DialogueManager dialogueManager;
    [SerializeField] PlayerEvents playerEvents;

    // *** DRIVER CODE
    // mainly Start() and Update()
    void Start()
    {
        // DIALOGUE MANAGER EVENTS
        dialogueManager.OnDialogueEnd += HandleOnDialogueEnd;
        // PLAYER EVENTS
        // -    movement
        playerEvents.OnAfterMove += HandlePlayerMovement;
        playerEvents.OnAimToggle += HandlePlayerAimToggle;

        /*
        playerEvents.OnAfterDealDamage += HandlePlayerAttack;
        playerEvents.OnAfterGetUpgrade += HandlePlayerUpgrade;*/


        // immediately starts with dialogue1
        PlayDialogue(tutorialFiles.tutorial_dialogue1);
        GameStateManager.Instance.currentState = GameState.Cutscene;
    }

    private void Update()
    {
        RunTutorialLogic();
    }
    // *** DRIVER CODE


    // *** CORE LOGIC
    // these are functions that coordinate smaller functions below
    async void RunTutorialLogic()
    {
        // imediately return if dialogue already active
        if (isDialogueActive)
            return;

        switch (tutorialIndex)
        {
            case 1: // -    MOVEMENT SEGMENT
                if (hasMovedN && hasMovedS && hasMovedE && hasMovedW)
                {
                    isDialogueActive = true;

                    await System.Threading.Tasks.Task.Delay(2000);
                    PlayDialogue(tutorialFiles.tutorial_movement);
                }
                break;

            case 2: // -    AIM TOGGLE SEGMENT
                if (hasAimActivated && hasAimDeactivated)
                {
                    isDialogueActive = true;

                    //await System.Threading.Tasks.Task.Delay(2000);
                    PlayDialogue(tutorialFiles.tutorial_aimAttack);
                }
                break;

            case 3: // -    AIM TOGGLE SEGMENT
                if (true)
                {
                    isDialogueActive = true;

                    await System.Threading.Tasks.Task.Delay(2000);
                    PlayDialogue(tutorialFiles.tutorial_upgrades);
                }
                break;

            default:
                break;
        }
    }
    // *** CORE LOGIC

    // *** SUB FUNCTIONS
    // more "individual" functions
    public void PlayDialogue(string fileName, GameState gameState = GameState.Cutscene)
    {
        TextAsset json = Resources.Load<TextAsset>("Dialogues/" + fileName);

        if (json == null)
        {
            Debug.LogError($"Dialogue '{fileName}' not found.");
            return;
        }

        DialogueFile dialogue =
            JsonUtility.FromJson<DialogueFile>(json.text);

        dialogueManager.StartDialogue(dialogue, gameState);
    }
    // *** SUB FUNCTIONS


    // TOOLS
    // non-function stuff like IEnumerator
    IEnumerator WaitTutorialPrompt()
    {
        yield return new WaitForSecondsRealtime(5f);
    }
    // TOOLS


    //  EVENTS & LISTENERS
    // put events and listeners here
    private void HandleOnDialogueEnd()
    {
        tutorialIndex++;
        isDialogueActive = false;

        Debug.Log($"DIALOGUE ENDED! tutorialIndex: {tutorialIndex}");
    }

    private void HandlePlayerMovement(MovementContext movementContext)
    {
        // detect inputs only when we're in the relevant tutorial segment!
        if (tutorialIndex != 1)
            return;


        if (movementContext.inputAxes.x > 0)
            hasMovedE = true;
        if (movementContext.inputAxes.x < 0)
            hasMovedW = true;

        if (movementContext.inputAxes.y > 0)
            hasMovedN = true;
        if (movementContext.inputAxes.y < 0)
            hasMovedS = true;
    }

    private void HandlePlayerAimToggle(AimContext aimContext)
    {
        if (tutorialIndex != 2)
            return;

        if (aimContext.isAiming)
            hasAimActivated = true;

        if (!aimContext.isAiming)
            hasAimDeactivated = true;
    }

    private void HandlePlayerAttack(DamageContext damageContext)
    {

    }

    private void HandlePlayerUpgrade()
    {

    }
    // EVENTS & LISTENERS
}

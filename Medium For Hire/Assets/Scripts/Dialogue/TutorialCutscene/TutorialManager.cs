using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] private bool hasMoved = false;
    [SerializeField] private bool hasAimed = false;


    [Header("REFERENCES")]
        // i stored the files in another class cuz its messy on the editor
    [SerializeField] TutorialFiles tutorialFiles;
    [SerializeField] DialogueManager dialogueManager;
    [SerializeField] PlayerEvents playerEvents;

    void Start()
    {
        // DIALOGUE MANAGER EVENTS
        dialogueManager.OnDialogueEnd += HandleOnDialogueEnd;

        // PLAYER EVENTS
        // -    movement
        /*playerEvents.OnAimToggle += HandlePlayerAimToggle;
        playerEvents.OnAfterDealDamage += HandlePlayerAttack;
        playerEvents.OnAfterGetUpgrade += HandlePlayerUpgrade;*/


        // immediately starts with dialogue1
        PlayDialogue(tutorialFiles.tutorial_dialogue1);
    }

    public void PlayDialogue(string fileName)
    {
        TextAsset json = Resources.Load<TextAsset>("Dialogues/" + fileName);

        if (json == null)
        {
            Debug.LogError($"Dialogue '{fileName}' not found.");
            return;
        }

        DialogueFile dialogue =
            JsonUtility.FromJson<DialogueFile>(json.text);

        dialogueManager.StartDialogue(dialogue);
        dialogueManager.DisplayCurrentLine();
    }

    private void HandleOnDialogueEnd()
    {
        // this activates as soon as a dialogue ends.
        // have this increment to the next "tutorial prompt"
        // -    aka, "wasd to move", followed by "RMB to toggle aim"

        // handle this by going down a series of if statements that break.
        // if (!hasMoved)
        // {
        //  doSomething();
        //  return;
        // }
        //
        // if (!hasAimed)
        // {
        //  ...
        //  return;
        // }
    }

    private void HandlePlayerMovement()
    {
        
    }

    private void HandlePlayerAttack(DamageContext damageContext)
    {

    }

    private void HandlePlayerAimToggle()
    {

    }

    private void HandlePlayerUpgrade()
    {

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Shrine : MonoBehaviour, IInteractable
{
    //public bool IsActivated { get; private set; } // once activated ('accepted superstition'), it despawns.
    //public string ShrineID { get; private set; }

        [Header("Shrine")]
    public bool IsActivated;
    public GameObject shrinePanel;
    public TMP_Text shrineName, superstitionName, superstitionDescription, rewardText, penaltyText;
    public GameObject interactiveText;

        [Header("Superstition Data")]
    public SuperstitionData superstitionData; // reference to the superstition data associated with this shrine

    void Start()
    {
        // HARDCODED 4 NOW
        shrineName.text = "The Shrine of Spirits";
        superstitionName.text = "Sukob";
        superstitionDescription.text = "Do not pick the same upgrade type twice in a row.";
        rewardText.text = "Next level-up offers 1 additional upgrade choice.";
        penaltyText.text = "Next level-up offers 1 less upgrade choice.";

        IsActivated = false;
    }

    void Update()
    {
        
    }
    public void Interact()
    {
        //if (!IsActivated) return;

        // open dialogue panel
        OpenDialoguePanel();
    }

    public void OnNotTouchingPlayer()
    {
        // hide 'E'
        if (interactiveText != null)
        {
            interactiveText.SetActive(false);
        }
    }

    public void OnTouchingPlayer()
    {
        // display 'E'
        if (interactiveText != null)
        {
            interactiveText.SetActive(true);
        }
    }

    public void OpenDialoguePanel()
    {
        // PAUSE TIME
        Time.timeScale = 0f;

        // display dialogue panel with superstition info
        shrinePanel.SetActive(true);

        // if 'Declined' button is clicked, call CloseDialoguePanel()

        // if 'Accepted' button is clicked, call SetActivated(true)
    }

    public void CloseDialoguePanel()
    {
        shrinePanel.SetActive(false);
        Time.timeScale = 1f; // RESUME TIME
        //SetActivated(true); // should be here, but temporarily wag muna
    }

    public void AcceptSuperstition()
    {
        SuperstitionManager.Instance.ActivateSuperstition(superstitionData);
        CloseDialoguePanel();
        SetActivated(true);
    }

    public void SetActivated(bool activated)
    {
        if (IsActivated = activated)
        {
            Destroy(gameObject); // shrine despawned
        }
    }
}

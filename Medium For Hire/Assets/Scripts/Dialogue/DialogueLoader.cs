using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueLoader : MonoBehaviour
{
    public DialogueManager dialogueManager;

    [Header("SETTINGS")]
    public bool startsDialogueImmediately;
    public string startingFile;

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

        Debug.Log(json.text);

        dialogueManager.StartDialogue(dialogue);
        dialogueManager.DisplayCurrentLine();
    }

    private void Start()
    {
        if (startsDialogueImmediately)
        {
            PlayDialogue(startingFile);
        }

    }
}

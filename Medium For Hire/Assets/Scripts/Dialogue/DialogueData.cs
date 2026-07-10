using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class DialogueFile
{
    public DialogueLine[] dialogueLines;

}

[Serializable]
public class DialogueLine
{
    public string id;

    public string illustration;

    public string portrait;
    public string bodyText;

    public string nameText;

}



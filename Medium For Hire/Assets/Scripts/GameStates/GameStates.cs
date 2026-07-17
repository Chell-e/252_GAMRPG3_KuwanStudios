using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public enum GameState
{
    Gameplay,           // gameplay only; game is running, all game inputs are accessible
    CombatPrompt,        // pop-up selection + tab + pause
    CombatDialogue,     // dialogue + tab + pause; dialogue navigation and tabbing

    Cutscene,           // dialogue ONLY; dialogue navigation

}


using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public enum GameState
{
    Gameplay, // default state; RMB to toggle, WASD to move, etc.
    Tabbed, // paused with TAB; display obtained upgrades, stats, etc disable gameplay controls
    SelectionPrompt, // superstition confirm, upgrade select; disable gameplay controls + pause game
    Cutscene // interactable dialogue popped up; disable gameplay controls + pause game
}


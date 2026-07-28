using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public enum GameState
{
    Gameplay, 
    Pause,         // ESC
    InfoTab,        // TAB
    Options,
    ShrinePanel,    // interact w/ shrine
    UpgradePanel,
    GameOver,

    Dialogue
}


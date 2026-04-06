using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SuperstitionData : ScriptableObject
{
    public string superstitionName;
    public string description;
    public string flavorText;

        // call when the stage starts
    public abstract void Initialize(StageManager stage);

        // call when the stage ends (cleans up events)
    public abstract void Deinitialize();

    protected void BreakRule(int count)
    {
            // notify superstition manager if the rule was broken
        SuperstitionManager.Instance.NotifyRuleBroken(this, count);
    }
}

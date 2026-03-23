using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseUpgradeRequirement : ScriptableObject
{
    public abstract bool IsAvailable();  // Abstract method to check the condition
}

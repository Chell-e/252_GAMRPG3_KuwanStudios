using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Superstitions/No Leftovers")]
public class NoLeftovers_Superstition : SuperstitionData
{
    public override void Initialize(StageManager stage)
    {
        ExpOrb.OnExpOrbExpire += HandleOrbExpired;
    }

    public override void Deinitialize()
    {
        ExpOrb.OnExpOrbExpire -= HandleOrbExpired;
    }

    private void HandleOrbExpired()
    {
        // break rule count +1
        BreakRule(1);
    }
}

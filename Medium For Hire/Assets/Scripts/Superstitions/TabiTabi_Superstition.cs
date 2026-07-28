using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SuperstitionData/Tabi tabi po")]
public class TabiTabi_Superstition : SuperstitionData
{
    [Header("Reward and Penalty")]
    public float hpToHealByShrine = 0.25f;
    public float shrineCooldownIncrease = 0.15f;

    [Header("REFERENCES")]
    private ShrineSpawner shrineSpawner;

    public override void Initialize(StageManager stage)
    {
        shrineSpawner = ShrineSpawner.Instance;

        PlayerController.OnDashWhileRestricted += HandleSuperstitionViolation;
    }

    // call when the stage ends (cleans up events)
    public override void Deinitialize()
    {
        PlayerController.OnDashWhileRestricted -= HandleSuperstitionViolation;
    }

    public override void ApplyReward()
    {
        if (shrineSpawner == null) return;

        shrineSpawner.allShrineHealAmount += (shrineSpawner.allShrineHealAmount * hpToHealByShrine);
    }

    public override void ApplyPenalty()
    {
        if (shrineSpawner == null) return;

        shrineSpawner.spiritCooldown += (shrineSpawner.spiritCooldown * shrineCooldownIncrease);
        shrineSpawner.akasiCooldown += (shrineSpawner.akasiCooldown * shrineCooldownIncrease);
        shrineSpawner.apolakiCooldown += (shrineSpawner.apolakiCooldown * shrineCooldownIncrease);
    }

    private void HandleSuperstitionViolation()
    {
        BreakRule(1);
    }

}
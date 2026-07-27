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
    private BaseShrine shrine;
    private ShrineSpawner shrineSpawner;

    public override void Initialize(StageManager stage)
    {
        shrine = BaseShrine.Instance;
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
        if (shrine == null) return;

        var finalHpToHealByShrine = shrine.healAmount;
        finalHpToHealByShrine += (hpToHealByShrine * finalHpToHealByShrine);
    }

    public override void ApplyPenalty()
    {
        if (shrine == null) return;

        var finalSpiritCooldown = shrineSpawner.spiritCooldown;
        var finalAkasiCooldown = shrineSpawner.akasiCooldown;
        var finalApolakiCooldown = shrineSpawner.apolakiCooldown;

        finalSpiritCooldown += (finalSpiritCooldown * shrineCooldownIncrease);
        finalAkasiCooldown += (finalAkasiCooldown * shrineCooldownIncrease);
        finalApolakiCooldown += (finalApolakiCooldown * shrineCooldownIncrease);
    }

    private void HandleSuperstitionViolation()
    {
        BreakRule(1);
    }

}
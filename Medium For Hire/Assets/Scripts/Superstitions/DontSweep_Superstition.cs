using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SuperstitionData/Dont Sweep")]
public class DontSweep_Superstition : SuperstitionData
{
    [Header("Reward and Penalty")]
    public float dashCooldownDecrease = 0.1f;
    public float dashCooldownIncrease = 0.2f;

    [Header("REFERENCES")]
    public PlayerController playerController;

    public override void Initialize(StageManager stage)
    {
        playerController = PlayerController.Instance.GetComponent<PlayerController>();

        ExpOrb.OnExpOrbCollectedWhileDashing += HandleSuperstitionViolation;
    }

    // call when the stage ends (cleans up events)
    public override void Deinitialize()
    {
        ExpOrb.OnExpOrbCollectedWhileDashing -= HandleSuperstitionViolation;
    }

    public override void ApplyReward()
    {
        if (playerController == null) return;

        var finalDashCooldown = playerController.dashCooldown;
        finalDashCooldown -= (finalDashCooldown * dashCooldownDecrease);
    }

    public override void ApplyPenalty()
    {
        if (playerController == null) return;

        var finalDashCooldown = playerController.dashCooldown;
        finalDashCooldown += (finalDashCooldown * dashCooldownIncrease);

    }

    private void HandleSuperstitionViolation()
    {
        //Debug.Log("player dashed while collecting an orb");
        BreakRule(1);
    }
}
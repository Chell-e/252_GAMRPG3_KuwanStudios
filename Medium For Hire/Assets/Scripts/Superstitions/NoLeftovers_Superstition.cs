using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(menuName = "SuperstitionData/No Leftovers")]
public class NoLeftovers_Superstition : SuperstitionData
{
    [Header("Reward and Penalty")]
    public int playerLevelCapIncrease = 5;
    public float expToLevelUpIncrease = 0.2f;

    [Header("REFERENCES")]
    public PlayerController playerController;
    public PlayerStats playerStats;

    public override void Initialize(StageManager stage)
    {
        playerController = PlayerController.Instance.GetComponent<PlayerController>();
        playerStats = PlayerStats.Instance.GetComponent<PlayerStats>();

        ExpOrb.OnExpOrbExpire += HandleOrbExpired;
    }

    public override void Deinitialize()
    {
        ExpOrb.OnExpOrbExpire -= HandleOrbExpired;
    }

    public override void ApplyReward()
    {
        if (PlayerController.Instance == null && PlayerStats.Instance == null) return;

        var maxLevel = playerStats.remainingLevels;
        maxLevel += playerLevelCapIncrease;
    }

    public override void ApplyPenalty()
    {
        if (PlayerController.Instance == null && PlayerStats.Instance == null) return;

        var updatedExpToLevel = (float)playerStats.expToLevel;
        updatedExpToLevel += expToLevelUpIncrease;

        Debug.Log($"Exp to next level up is now {playerStats.expToLevel + playerStats.currentLevel}");
    }

    private void HandleOrbExpired()
    {
        // break rule count +1
        BreakRule(1);
    }
}

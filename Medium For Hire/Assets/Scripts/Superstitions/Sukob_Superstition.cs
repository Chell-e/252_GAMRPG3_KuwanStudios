using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "Superstitions/Sukob")]
public class Sukob_Superstition : SuperstitionData
{
    private string lastDomainPicked = "";

    public override void Initialize(StageManager stage)
    {
        // reset
        lastDomainPicked = "";

        UpgradeManager.Instance.OnOffenseDomainUpgradeChosen += HandleOffenseDomain;
        UpgradeManager.Instance.OnSurvivalDomainUpgradeChosen += HandleSurvivalDomain;
        UpgradeManager.Instance.OnUtilityDomainUpgradeChosen += HandleUtilityDomain;
    }

    public override void Deinitialize()
    {
        if (UpgradeManager.Instance != null)
        {
            UpgradeManager.Instance.OnOffenseDomainUpgradeChosen -= HandleOffenseDomain;
            UpgradeManager.Instance.OnSurvivalDomainUpgradeChosen -= HandleSurvivalDomain;
            UpgradeManager.Instance.OnUtilityDomainUpgradeChosen -= HandleUtilityDomain;
        }
    }

    private void HandleOffenseDomain() => CheckConsecutive("Offense");
    private void HandleSurvivalDomain() => CheckConsecutive("Survival");
    private void HandleUtilityDomain() => CheckConsecutive("Utility");

    private void CheckConsecutive(string currentDomain)
    {
        if (currentDomain == lastDomainPicked)
        {
            Debug.Log("RULE BROKEN: Chose " + lastDomainPicked + " last time, chose " + currentDomain + " this time");
            BreakRule(1);
        }

        lastDomainPicked = currentDomain;
    }
}

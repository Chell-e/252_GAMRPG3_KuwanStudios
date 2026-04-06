using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TooltipTrigger))]
public class SimpleStatTooltip : MonoBehaviour , ITooltipProvider
{
    [SerializeField] Stat statForTooltip;

    private TooltipTrigger tooltipTrigger;

    private void Start()
    {
        tooltipTrigger = this.GetComponent<TooltipTrigger>();
        tooltipTrigger.SetProvider(this);
    }

    public virtual string GetTooltipText()
    {
        string tooltipString;
        PlayerStats playerStats = PlayerStats.Instance;

        tooltipString = statForTooltip.ToString()
            + ": " + PlayerStats.Instance.GetPlayerStat(statForTooltip);


        switch (statForTooltip)
        {
            case (Stat.DomainOffense):
                tooltipString = "<sprite name=\"grudge\"> Ancestral Grudge: " + playerStats.GetPlayerStat(statForTooltip);
                break;
            case (Stat.DomainSurvival):
                tooltipString = "<sprite name=\"guard\"> Ancestral Guardian: " + playerStats.GetPlayerStat(statForTooltip);
                break;
            case (Stat.DomainUtility):
                tooltipString = "<sprite name=\"guide\"> Ancestral Guidance: " + playerStats.GetPlayerStat(statForTooltip);
                break;
            default:
                break;
        }

        

        return tooltipString;
    }

    public virtual string GetName()
    {
        switch (statForTooltip)
        {
            case Stat.DomainOffense:
                return "Ancestral Grudge";
            case Stat.DomainSurvival:
                return "Ancestral Guardian";
            case Stat.DomainUtility:
                return "Ancestral Guidance";
            default:
                return statForTooltip.ToString();
        }
    }

    public virtual string GetDescription()
    {
        switch (statForTooltip)
        {
            case Stat.DomainOffense:
                return "The fury of your ancestral spirit manifests as physical power, increasing your weapon's damage.";
            case Stat.DomainSurvival:
                return "The spirit of your ancestor shields you from harm, increasing your survivability.";
            case Stat.DomainUtility:
                return "Your ancestral spirit guides your path.";
            default:
                return statForTooltip.ToString();

        }
    }
}

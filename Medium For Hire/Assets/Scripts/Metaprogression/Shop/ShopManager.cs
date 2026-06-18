using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    [Header("Speech Bubble UI")]
    public GameObject speechBubble;
    public GameObject greeting;
    public TextMeshProUGUI shopInfoDialogue;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI costText;
    public Button buyButton;

    [Header("Pilon Display")]
    public TextMeshProUGUI pilonBalanceText;

    private string selectedStat;

    void Start()
    {
        speechBubble.SetActive(false);
        UpdateBalanceUI();
    }

    public void UpdateBalanceUI()
    {
        pilonBalanceText.text = PlayerData.Instance.pilonAmount.ToString();
    }

    public void OnStatIconClicked(string statName)
    {
        selectedStat = statName;
        speechBubble.SetActive(true);
        greeting.SetActive(false);

        int currentLvl = GetStatLevel(statName);
        int bonus = GetStatBonus(statName);
        int cost = GetCost(statName, currentLvl);

        shopInfoDialogue.text = "It can boost your " + statName + " by +" + bonus + "% permanently. Want to buy it?";
        levelText.text = "Current Level: " + currentLvl + "/5";

        if (currentLvl >= 5)
        {
            costText.text = "FULLY UPGRADED";
            buyButton.interactable = false;
        }
        else
        {
            costText.text = cost.ToString(); ;
            buyButton.interactable = PlayerData.Instance.pilonAmount >= cost;
        }
    }

    public void ConfirmPurchase()
    {
        int currentLvl = GetStatLevel(selectedStat);
        int cost = GetCost(selectedStat, currentLvl);

        if (PlayerData.Instance.pilonAmount >= cost && currentLvl < 5)
        {
            PlayerData.Instance.pilonAmount -= cost;

            if (selectedStat == "Health") PlayerData.Instance.healthLevel++;
            else if (selectedStat == "Damage") PlayerData.Instance.damageLevel++;
            else if (selectedStat == "AtkSpeed") PlayerData.Instance.attackSpeedLevel++;
            else if (selectedStat == "MoveSpeed") PlayerData.Instance.moveSpeedLevel++;
            else if (selectedStat == "ProjSpeed") PlayerData.Instance.projectileSpeedLevel++;
            else if (selectedStat == "Pickup") PlayerData.Instance.pickupRangeLevel++;

            SaveDataJSON.Instance.SaveData();
            UpdateBalanceUI();
            OnStatIconClicked(selectedStat); 
        }
    }

    private int GetStatLevel(string s)
    {
        if (s == "Health") return PlayerData.Instance.healthLevel;
        if (s == "Damage") return PlayerData.Instance.damageLevel;
        if (s == "AtkSpeed") return PlayerData.Instance.attackSpeedLevel;
        if (s == "MoveSpeed") return PlayerData.Instance.moveSpeedLevel;
        if (s == "ProjSpeed") return PlayerData.Instance.projectileSpeedLevel;
        if (s == "Pickup") return PlayerData.Instance.pickupRangeLevel;
        return 0;
    }

    private int GetStatBonus(string s)
    {
        if (s == "Health") return PlayerStats.healthBonusPerLevel;
        if (s == "Damage") return PlayerStats.dmgBonusPerLevel;
        if (s == "AtkSpeed") return PlayerStats.atkSpeedBonusPerLevel;
        if (s == "MoveSpeed") return PlayerStats.moveSpeedBonusPerLevel;
        if (s == "ProjSpeed") return PlayerStats.projSpeedBonusPerLevel;
        if (s == "Pickup") return PlayerStats.pickupRangeBonusPerLevel;
        return 0;
    }

    private int GetCost(string s, int lvl)
    {
        if (s == "Damage" || s == "AtkSpeed") return 150 + (lvl * 150);
        if (s == "Health" || s == "MoveSpeed") return 100 + (lvl * 100);
        return 75 + (lvl * 75);
    }
}
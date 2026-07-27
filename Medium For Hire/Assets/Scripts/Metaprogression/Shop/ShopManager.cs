using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    [Header("DESCRIPTION BOX UI")]
    public TextMeshProUGUI itemName;
    public TextMeshProUGUI itemDescription;
    public TextMeshProUGUI itemLevel;
    public TextMeshProUGUI cost;
    public Button buyButton;

    [Header("CURRENCY UI")]
    public TextMeshProUGUI tornPagesAmountText;

    private string selectedStat;

    void Start()
    {
        UpdateBalanceUI();
    }

    public void UpdateBalanceUI()
    {
        tornPagesAmountText.text = PlayerData.Instance.tornPagesAmount.ToString();
    }

    public void OnStatIconClicked(string statName)
    {
        selectedStat = statName;

        int currentLvl = GetStatLevel(statName);
        int bonus = GetStatBonus(statName);
        int cost = GetCost(statName, currentLvl);

        itemName.text = statName;
        itemDescription.text = "Boosts your " + statName + " by +" + bonus + "%";
        itemLevel.text = currentLvl + "/3";

        if (currentLvl >= 5)
        {
            this.cost.text = "Fully Upgraded";
            buyButton.interactable = false;
        }
        else
        {
            this.cost.text = cost.ToString(); ;
            buyButton.interactable = PlayerData.Instance.tornPagesAmount >= cost;
        }
    }

    public void ConfirmPurchase()
    {
        int currentLvl = GetStatLevel(selectedStat);
        int cost = GetCost(selectedStat, currentLvl);

        if (PlayerData.Instance.tornPagesAmount >= cost && currentLvl < 3)
        {
            PlayerData.Instance.tornPagesAmount -= cost;

            if (selectedStat == "Health") PlayerData.Instance.healthLevel++;
            else if (selectedStat == "Damage") PlayerData.Instance.damageLevel++;
            else if (selectedStat == "Attack Speed") PlayerData.Instance.attackSpeedLevel++;
            else if (selectedStat == "Move Speed") PlayerData.Instance.moveSpeedLevel++;
            else if (selectedStat == "Projectile Speed") PlayerData.Instance.projectileSpeedLevel++;
            else if (selectedStat == "Pickup Range") PlayerData.Instance.pickupRangeLevel++;

            SaveDataJSON.Instance.SaveData();
            UpdateBalanceUI();
            OnStatIconClicked(selectedStat); 
        }
    }

    private int GetStatLevel(string s)
    {
        if (s == "Health") return PlayerData.Instance.healthLevel;
        if (s == "Damage") return PlayerData.Instance.damageLevel;
        if (s == "Attack Speed") return PlayerData.Instance.attackSpeedLevel;
        if (s == "Move Speed") return PlayerData.Instance.moveSpeedLevel;
        if (s == "Projectile Speed") return PlayerData.Instance.projectileSpeedLevel;
        if (s == "Pickup Range") return PlayerData.Instance.pickupRangeLevel;
        return 0;
    }

    private int GetStatBonus(string s)
    {
        if (s == "Health") return PlayerStats.healthBonusPerLevel;
        if (s == "Damage") return PlayerStats.dmgBonusPerLevel;
        if (s == "Attack Speed") return PlayerStats.atkSpeedBonusPerLevel;
        if (s == "Move Speed") return PlayerStats.moveSpeedBonusPerLevel;
        if (s == "Projectile Speed") return PlayerStats.projSpeedBonusPerLevel;
        if (s == "Pickup Range") return PlayerStats.pickupRangeBonusPerLevel;
        return 0;
    }

    private int GetCost(string s, int lvl)
    {
        if (s == "Damage" || s == "Attack Speed") return 20 + (lvl * 3);
        if (s == "Health" || s == "Move Speed") return 15 + (lvl * 2);
        return 10 + (lvl * 2);
    }
}
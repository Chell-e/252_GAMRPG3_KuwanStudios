using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Misc;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance;

        [Header("Pool & UI")]
    [SerializeField] public BaseUpgradeData[] normalUpgradePool; // this data type supports both StatUpgrade and WeaponUnlock
    [SerializeField] public BaseUpgradeData[] specialUpgradePool;
    public GameObject cardPrefab;
    public Transform cardContainer;

        [Header("Options")]
    [SerializeField] public int cardsToShow = 3;


        [Header("Debug")]
    private bool isOpen = false;
    private List<GameObject> spawnedCards = new List<GameObject>();

    // track how many times we picked specific upgrades
    private Dictionary<BaseUpgradeData, int> totalCardsPicked = new Dictionary<BaseUpgradeData, int>(); 

    // for making SINGLETON
    private void Awake() 
    {
        if (Instance != null && Instance != this) Destroy(this);
        else Instance = this;
    }

    public void ShowUpgradeOptions(bool isSpecial)
    {
        // avoid duplicating UI
        if (isOpen) return;

        // return if null
        if (normalUpgradePool == null || normalUpgradePool.Length == 0 || cardPrefab == null || cardContainer == null)
        {
            Debug.LogWarning("UpgradeManager: missing references or empty pool.");
            return;
        }


        // clamp the upgrades shown between 3 and remaining pool
        int displayedCardsCount = Mathf.Clamp(cardsToShow, 0 , normalUpgradePool.Length);

        var rolledUpgrades = RollNormalUpgrades();
        if (isSpecial) rolledUpgrades = RollSpecialUpgrades();

        // Pause game
        Time.timeScale = 0f;
        isOpen = true;

        // instantiate cards
        foreach (var upgrade in rolledUpgrades)
        {
            var cardObject = Instantiate(cardPrefab, cardContainer);
            // ensure correct scale so layout groups size/position correctly
            cardObject.transform.localScale = Vector3.one;
            
            var cardUI = cardObject.GetComponent<UpgradeCardUI>();
            if (cardUI != null) cardUI.Setup(upgrade, OnCardSelected);

            spawnedCards.Add(cardObject);
        }

        // Force the layout system to update immediately (important when Time.timeScale == 0)
        var rt = cardContainer as RectTransform;
        if (rt != null)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(rt);
        }
    }

    private List<BaseUpgradeData> RollNormalUpgrades()
    {
        int upgradesToRoll = Mathf.Clamp(cardsToShow, 0, normalUpgradePool.Length); // how many upgrades

        // copy the entire normalUpgradePool
        var allNormalUpgrades = normalUpgradePool.ToList();

        // prepare a list of upgrades to be taken from the copy
        var rolledUpgrades = new List<BaseUpgradeData>();

        for (int i = 0; i < upgradesToRoll; i++)
        {
            int rolledIndex = Random.Range(0, allNormalUpgrades.Count); // roll a random upgrade

            int amountPicked = 0;
            totalCardsPicked.TryGetValue(allNormalUpgrades[rolledIndex], out amountPicked); // check if this upgrade has been picked before
            
            if (amountPicked < allNormalUpgrades[rolledIndex].maxPicks || allNormalUpgrades[rolledIndex].maxPicks == -1)
            {
                rolledUpgrades.Add(allNormalUpgrades[rolledIndex]);

                if (allNormalUpgrades[rolledIndex].canDuplicate == false)
                    allNormalUpgrades.RemoveAt(rolledIndex); // remove if the upgrade doesn't dupe
            }
            else // if exceeded picks, remove this card
            {
                allNormalUpgrades.RemoveAt(rolledIndex);
            }
            
        }

        return rolledUpgrades;
    }

    private List<BaseUpgradeData> RollSpecialUpgrades()
    {
        int upgradesToRoll = Mathf.Clamp(cardsToShow, 0, specialUpgradePool.Length); // how many upgrades

        // copy the entire specialUpgradePool
        var allSpecialUpgrades = specialUpgradePool.ToList();

        // prepare a list of upgrades to be taken from the copy
        var rolledUpgrades = new List<BaseUpgradeData>();

        for (int i = 0; i < upgradesToRoll; i++)
        {
            int rolledIndex = Random.Range(0, allSpecialUpgrades.Count); // roll a random upgrade

            int amountPicked = 0;
            totalCardsPicked.TryGetValue(allSpecialUpgrades[rolledIndex], out amountPicked); // check if this upgrade has been picked before

            if (amountPicked < allSpecialUpgrades[rolledIndex].maxPicks || allSpecialUpgrades[rolledIndex].maxPicks == -1)
            {
                rolledUpgrades.Add(allSpecialUpgrades[rolledIndex]);

                if (allSpecialUpgrades[rolledIndex].canDuplicate == false)
                    allSpecialUpgrades.RemoveAt(rolledIndex); // remove if the upgrade doesn't dupe
            }
            else // if exceeded picks, remove this card
            {
                allSpecialUpgrades.RemoveAt(rolledIndex);
                i -= 1;
            }

        }

        return rolledUpgrades;
    }

    private void OnCardSelected(BaseUpgradeData upgrade)
    {
        ApplyUpgrade(upgrade);
        CloseAndCleanup();
    }

    private void ApplyUpgrade(BaseUpgradeData upgrade)
    {
        if (PlayerController.Instance == null || PlayerController.Instance.playerStats == null)
        {
            Debug.LogWarning("UpgradeManager: no player stats to apply upgrade to.");
            return;
        }
        //================

        // Update the totalCardsPicked dictionary
        if (totalCardsPicked.ContainsKey(upgrade) )
            totalCardsPicked[upgrade]++;
        else
            totalCardsPicked[upgrade] = 1;

            // remove this later
        Debug.Log("PICKS:");
        foreach (KeyValuePair<BaseUpgradeData, int> cardPicked in totalCardsPicked)
        {
            Debug.Log(cardPicked);
        }
            // remove this later

        var playerStats = PlayerController.Instance.playerStats;
        var playerHealth = PlayerController.Instance.GetComponent<HealthComponent>();



        if (upgrade is StatUpgrade statUpgrade) // the "is" keyword checks for a specific derived type
        {
            var targetedStats = statUpgrade.statsUpgraded;

            foreach (var stat in targetedStats)
            {
                switch (stat.statToUpgrade) // *********************** ENCAPSULATION: SET PLAYER STATS TO PRIVATE LATER.
                {
                    case StatUpgradeType.DamagePercent:
                        playerStats.dmgPercent += stat.value;
                        break;
                    case StatUpgradeType.AttackSpeedPercent:
                        playerStats.atkSpeedPercent += stat.value;
                        break;

                    case StatUpgradeType.MaxHealthPercent:
                        playerStats.maxHealthPercent += stat.value;
                        break;
                    case StatUpgradeType.MoveSpeedPercent:
                        playerStats.movespeedPercent += stat.value;
                        break;

                    case StatUpgradeType.ProjectileSpeedPercent:
                        playerStats.projectileSpeedPercent += stat.value;
                        break;

                    case StatUpgradeType.Heal:
                        playerHealth.Heal(stat.value);
                        break;


                    case StatUpgradeType.OffenseBonus:
                        playerStats.offenseDomainStat += stat.value;
                        break;
                    case StatUpgradeType.SurvivalBonus:
                        playerStats.survivalDomainStat += stat.value;
                        break;
                    case StatUpgradeType.UtilityBonus:
                        playerStats.utilityDomainStat += stat.value;
                        break;


                    default:
                        Debug.Log($"UpgradeManager: unhandled upgrade type {stat.statToUpgrade}");
                        break;
                }

            }

        }

        if (upgrade is WeaponUnlock weaponUnlock)
        {
            // add new weapon to player
        }

        if (upgrade is WeaponEvolution weaponEvolution)
        {
            // weapon evolution logic here
        }


            // update any relevant UI
        UIManager.Instance.UpdateExpSlider();
        UIManager.Instance.UpdateHpSlider();

            // update player
        playerStats.UpdatePlayerStat(Stat.MaxHealth);

    }

    private void CloseAndCleanup()
    {
        // destroy spawned card GameObjects
        foreach (var cardToRemove in spawnedCards) if (cardToRemove != null) Destroy(cardToRemove);
        spawnedCards.Clear();

        // unpause
        Time.timeScale = 1f;
        isOpen = false;
    }
}

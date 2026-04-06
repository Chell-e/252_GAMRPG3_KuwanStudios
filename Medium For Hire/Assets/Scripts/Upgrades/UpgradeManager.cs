using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Misc;

public enum UpgradePool
{
    Normal,
    Special
}

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance;
    private void Awake() // for SINGLETON
    {
        // singleton 
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }


        [Header("Pool & UI")]
    [SerializeField] public BaseUpgradeData[] normalUpgradePool; // this data type supports both StatUpgrade and WeaponUnlock
    [SerializeField] public BaseUpgradeData[] specialUpgradePool;
    public GameObject cardPrefab;
    public Transform cardContainer;


        [Header("Options")]
    [SerializeField] public int cardsToShow = 3;


        [Header("Debug")]
    private bool isOpen = false;
    private List<GameObject> spawnedCards = new List<GameObject>(); // currently displayed cards


    // track how many times we picked specific upgrades
    private Dictionary<BaseUpgradeData, int> totalCardsPicked = new Dictionary<BaseUpgradeData, int>();


    public System.Action OnOffenseDomainUpgradeChosen;
    public System.Action OnSurvivalDomainUpgradeChosen;
    public System.Action OnUtilityDomainUpgradeChosen;


    public void ShowUpgradeOptions(bool isSpecial)
    {
        // avoid duplicating UI
        if (isOpen) return;

        /*// return if null
        if (normalUpgradePool == null || normalUpgradePool.Length == 0 || cardPrefab == null || cardContainer == null)
        {
            Debug.LogWarning("UpgradeManager: missing references or empty pool.");
            return;
        }*/


        UIManager.Instance.ToggleUpgradeGraphics(true);

        List<BaseUpgradeData> rolledUpgrades; // list of upgrades that qualify
        if (isSpecial)
            rolledUpgrades = RollUpgrades(UpgradePool.Special);
        else
            rolledUpgrades = RollUpgrades(UpgradePool.Normal);
        
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

    private List<BaseUpgradeData> RollUpgrades(UpgradePool upgradePoolType)
    {
        Debug.Log(upgradePoolType);
        
        var upgradeListOfType =     // make a copy of the upgrade pool specified
            (upgradePoolType == UpgradePool.Normal)
            ? normalUpgradePool.ToList()
            : specialUpgradePool.ToList();

        int upgradesToRoll = Mathf.Clamp(cardsToShow, 0, upgradeListOfType.Count); // determine how many upgrades we're rolling

        // first filter out which upgrades we actually have access to
        List<BaseUpgradeData> viableUpgradeList = new List<BaseUpgradeData>();  // prepare list of upgrades
        foreach (var upgrade in upgradeListOfType)      // iterate thru the copied pool
        {
            bool upgradeViability = true;
            foreach (var requirement in upgrade.requirements)
            {
                if (!requirement.IsAvailable())
                {
                    upgradeViability = false;
                    Debug.Log(upgrade + " does not meet condition: " + requirement);
                }
                    
            }

            if (upgradeViability == true)
                viableUpgradeList.Add(upgrade);

        }


        var rolledUpgrades = new List<BaseUpgradeData>();

        for (int i = 0; i < upgradesToRoll; i++)
        {
            int rolledIndex = Random.Range(0, viableUpgradeList.Count); // roll a random upgrade

            int amountPicked = 0;
            totalCardsPicked.TryGetValue(viableUpgradeList[rolledIndex], out amountPicked); // check if this upgrade has been picked before

            if (amountPicked < viableUpgradeList[rolledIndex].maxPicks || viableUpgradeList[rolledIndex].maxPicks == -1)
            {
                rolledUpgrades.Add(viableUpgradeList[rolledIndex]);

                if (viableUpgradeList[rolledIndex].canDuplicate == false)
                    viableUpgradeList.RemoveAt(rolledIndex); // remove if the upgrade doesn't dupe
            }
            else // if exceeded picks, remove this card
            {
                viableUpgradeList.RemoveAt(rolledIndex);
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
                        playerHealth.IncreaseMaxHealth(Mathf.RoundToInt(playerHealth.GetMaxHealth() * stat.value));
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
                        OnOffenseDomainUpgradeChosen?.Invoke();
                        break;
                    case StatUpgradeType.SurvivalBonus:
                        playerStats.survivalDomainStat += stat.value;
                        OnSurvivalDomainUpgradeChosen?.Invoke();
                        break;
                    case StatUpgradeType.UtilityBonus:
                        playerStats.utilityDomainStat += stat.value;
                        OnUtilityDomainUpgradeChosen?.Invoke();
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

            if (weaponUnlock.weaponPrefab == null)
            {
                Debug.Log("weaponPrefab is null.");
                return;
            }

            BaseWeapon newWeaponInstance = PlayerController.Instance.weaponManager.AddMiniWeapon(weaponUnlock.weaponPrefab);
            Debug.Log("Added weapon: " + weaponUnlock);

            UIManager.Instance.AddWeaponSlot(weaponUnlock, newWeaponInstance);
        }

        if (upgrade is WeaponEvolution weaponEvolution)
        {
            // weapon evolution logic here
        }


            // update any relevant UI
        UIManager.Instance.UpdateExpUI();
        UIManager.Instance.UpdateHpUI();
        UIManager.Instance.UpdateDomainProgress();

        // update player
        playerStats.UpdatePlayerStat(Stat.MaxHealth);

    }

    private void CloseAndCleanup()
    {
        // destroy spawned card GameObjects
        foreach (var cardToRemove in spawnedCards) if (cardToRemove != null) Destroy(cardToRemove);
        spawnedCards.Clear();

        // remove upgrades from UI Controller 
        UIManager.Instance.ToggleUpgradeGraphics(false);

        // unpause
        Time.timeScale = 1f;
        isOpen = false;
    }
}

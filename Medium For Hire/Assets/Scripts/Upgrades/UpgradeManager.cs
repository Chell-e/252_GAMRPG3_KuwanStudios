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

    [SerializeField] public BaseUpgradeData[] uniqueUpgradePool; // only separated from special for convenience
    
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


        // SHOW BACKGROUND GRAPHICS
        UIManager.Instance.ToggleUpgradeGraphics(true); 
        // SHOW BACKGROUND GRAPHICS


        // ROLL QUALIFYING UPGRADES
        List<BaseUpgradeData> rolledUpgrades; // list of upgrades that qualify
        if (isSpecial)
            rolledUpgrades = RollUpgrades(UpgradePool.Special);
        else
            rolledUpgrades = RollUpgrades(UpgradePool.Normal);
        // ROLL QUALIFYING UPGRADES


        // Pause game
        Time.timeScale = 0f;
        isOpen = true;
        // PAUSE GAME


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
        
        
        // COPY LISTS TO DRAW FROM
        var upgradeListOfType =     // make a copy of the upgrade pool specified
            (upgradePoolType == UpgradePool.Normal)
            ? normalUpgradePool.ToList()
            : specialUpgradePool.ToList();

        List<BaseUpgradeData> upgradeListOfPriority = new List<BaseUpgradeData>();
        if (upgradePoolType == UpgradePool.Special)
            upgradeListOfPriority = uniqueUpgradePool.ToList(); // need to make ALL qualifying unique upgrades to show up.
        // COPY LISTS TO DRAW FROM


        // FILTER OUT WHICH UPGRADES ARE ACCESSIBLE
        List<BaseUpgradeData> viableUpgradeList = FilterUpgradesByAvailability(upgradeListOfType); // iterate thru the copied pool
        List<BaseUpgradeData> viablePriorities = FilterUpgradesByAvailability(upgradeListOfPriority);
        // FILTER OUT WHICH UPGRADES ARE ACCESSIBLE


        // RANDOMLY PICK FROM ACCESSIBLE UPGRADES
        int upgradesToRoll = Mathf.Clamp(cardsToShow, 0, upgradeListOfType.Count); // determine how many upgrades we're rolling
        var rolledUpgrades = new List<BaseUpgradeData>();

        Debug.Log("# of upgrades to roll: " + upgradesToRoll);

        PopulateUpgradeListRandomly(rolledUpgrades, viablePriorities, upgradesToRoll); // populate with priorities first
        PopulateUpgradeListRandomly(rolledUpgrades, viableUpgradeList, upgradesToRoll);

        return rolledUpgrades;
    }

    private List<BaseUpgradeData> FilterUpgradesByAvailability(List<BaseUpgradeData> _sourceUpgradeList)
    {
        List<BaseUpgradeData> filteredUpgrades = new List<BaseUpgradeData>();

        foreach (var upgrade in _sourceUpgradeList)      // iterate thru the copied pool
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
                filteredUpgrades.Add(upgrade);

        }

        return filteredUpgrades;

    }

    private void PopulateUpgradeListRandomly(List<BaseUpgradeData> _targetList, List<BaseUpgradeData> _sourceList, int _maxItems)
    {
        int maxItemsRemaining = _maxItems - _targetList.Count; // check if _targetList actually still has space for items
        if (maxItemsRemaining <= 0) return;

        List<BaseUpgradeData> copyOfSource = _sourceList.ToList();
        if (copyOfSource.Count <= 0) return; // return early if the source is empty...

        foreach (var thing in copyOfSource)
            Debug.Log(thing);

        //Debug.Log("max items remaining: " + maxItemsRemaining);
        for (int i = 0; i < maxItemsRemaining; i++)
        {
            if (copyOfSource.Count <= 0) return; // return when source is empty


            int rolledIndex = Random.Range(0, copyOfSource.Count); // roll a random upgrade

            //Debug.Log("rolled index is: " + rolledIndex);

            int amountPicked = 0;
            totalCardsPicked.TryGetValue(copyOfSource[rolledIndex], out amountPicked); // check if this upgrade has been picked before

            if (amountPicked < copyOfSource[rolledIndex].maxPicks
                || copyOfSource[rolledIndex].maxPicks == -1)
            {
                _targetList.Add(copyOfSource[rolledIndex]);

                if (copyOfSource[rolledIndex].canDuplicate == false)
                    copyOfSource.RemoveAt(rolledIndex); // remove if the upgrade doesn't dupe
            }
            else // if exceeded picks, remove this card
            {
                copyOfSource.RemoveAt(rolledIndex);
                i -= 1;
            }

        }

        // no return, this should directly modify _targetList

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
                    // OFFENSE
                    case StatUpgradeType.DamagePercent:
                        playerStats.dmgPercent += stat.value;
                        break;
                    case StatUpgradeType.AttackSpeedPercent:
                        playerStats.atkSpeedPercent += stat.value;
                        break;

                    // SURVIVAL
                    case StatUpgradeType.MaxHealthPercent:
                        playerStats.maxHealthPercent += stat.value;
                        playerHealth.IncreaseMaxHealth(Mathf.RoundToInt(playerHealth.GetMaxHealth() * stat.value));
                        break;
                    case StatUpgradeType.MoveSpeedPercent:
                        playerStats.movespeedPercent += stat.value;
                        break;

                    // UTILITY
                    case StatUpgradeType.ProjectileSpeedPercent:
                        playerStats.projectileSpeedPercent += stat.value;
                        break;
                    case StatUpgradeType.AreaPercent:
                        playerStats.areaPercent += stat.value;
                        break;
                    case StatUpgradeType.PickupRangePercent:
                        playerStats.pickupRangePercent += stat.value;
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
            var weaponManager = PlayerController.Instance.weaponManager;
            switch (weaponEvolution.evolutionType)
            {
                case WeaponEvolutionType.OffenseEvolution:
                    weaponManager.mainWeapon.GetComponent<BaseWeapon>()
                        .EvolveOffense();
                    break;

                case WeaponEvolutionType.SurvivalEvolution:
                    weaponManager.mainWeapon.GetComponent<BaseWeapon>()
                        .EvolveSurvival();
                    break;

                case WeaponEvolutionType.UtilityEvolution:
                    weaponManager.mainWeapon.GetComponent<BaseWeapon>()
                        .EvolveUtility();
                    break;


                default:
                    Debug.Log("Unsupported evolution type");
                    break;
            }

            // weapon evolution logic here
        }


            // update any relevant UI
        UIManager.Instance.UpdateExpUI();
        UIManager.Instance.UpdateHpUI();
        UIManager.Instance.UpdateDomainProgress();

        PlayerStats.Instance.DoScaleStats();

        /*EVENT*/ PlayerController.Instance.Events.OnAfterGetUpgrade?.Invoke();
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

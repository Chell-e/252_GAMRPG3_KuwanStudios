using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    // singleton stuff
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
    // singleton stuff

    [Header("SETTINGS")]
    public int handSize;

    [Header("DEBUG")]
    private List<GameObject> displayedCards = new List<GameObject>(); // currently displayed cards

    [Header("REFERENCES")]
    public UpgradePool upgradePool;
    public UpgradeRoller Roller;
    public AcquiredUpgrades Acquired;
    [Space]
    public Transform cardContainer;
    public GameObject cardPrefab;



    // * DRIVER CODE
    private void Start()
    {
        //StartUpgradeSelect();

        //List<BaseUpgradeData> hand = Roller.BulkRoll(Acquired.acquiredUpgrades, handSize);

        //foreach (BaseUpgradeData card in hand)
        //{
        //    //Debug.Log(card.title);
        //    DisplayCard(card);
        //}

    }

    [ContextMenu("Force Level-Up")]
    public void StartUpgradeSelect()
    {
        GameStateManager.Instance?.SetState(GameState.Cutscene);
        UIManager.Instance?.ToggleUpgradeGraphics(true);

        DrawHand();
    }

    public void EndUpgradeSelect()
    {
        ClearHand();

        UIManager.Instance?.ToggleUpgradeGraphics(false);
        GameStateManager.Instance?.ReturnState();
    }

    [ContextMenu("Reroll Hand")]
    public void RerollHand()
    {
        ClearHand();
        DrawHand();
    }
    // * DRIVER CODE


    // *** CORE LOGIC
    private void ApplyUpgrade(BaseUpgradeData upgrade)
    {
        IncrementUpgrade(upgrade);

        if (upgrade is StatUpgrade statUpgrade) // the "is" keyword checks for a specific derived type
            ApplyUpgrade_Stats(statUpgrade);

        if (upgrade is WeaponUnlock weaponUnlock)
            ApplyUpgrade_WeaponUnlock(weaponUnlock);

        if (upgrade is WeaponEvolution weaponEvolution)
            ApplyUpgrade_WeaponEvolution(weaponEvolution);



        // update any relevant UI
        UIManager.Instance.UpdateExpUI();
        UIManager.Instance.UpdateHpUI();
        UIManager.Instance.UpdateDomainProgress();

        PlayerStats.Instance.DoScaleStats();

        /*EVENT*/
        PlayerController.Instance.Events.OnAfterGetUpgrade?.Invoke();
    }
    // *** CORE LOGIC


    // ** SUB FUNCTIONS
    private void ApplyUpgrade_Stats(StatUpgrade statUpgrade)
    {
        var targetedStats = statUpgrade.statsUpgraded;

        var playerStats = PlayerController.Instance.playerStats;
        var playerHealth = PlayerController.Instance.GetComponent<HealthComponent>();


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
                    Debug.Log("maxHealth stat.value: " + stat.value);
                    Debug.Log("IncreaseMaxHealth: " + Mathf.RoundToInt(playerHealth.GetMaxHealth() * stat.value));
                    playerStats.maxHealthPercent += stat.value;

                    // make sure maxHealthPercent multiplies the maxHealth base value.
                    //playerHealth.IncreaseMaxHealth(Mathf.RoundToInt(playerHealth.GetMaxHealth() * stat.value )); 

                    // this is applied as a flat bonus for now
                    playerHealth.IncreaseMaxHealth(stat.value);
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
                    //OnOffenseDomainUpgradeChosen?.Invoke();
                    break;
                case StatUpgradeType.SurvivalBonus:
                    playerStats.survivalDomainStat += stat.value;
                    //OnSurvivalDomainUpgradeChosen?.Invoke();
                    break;
                case StatUpgradeType.UtilityBonus:
                    playerStats.utilityDomainStat += stat.value;
                    //OnUtilityDomainUpgradeChosen?.Invoke();
                    break;


                default:
                    Debug.Log($"UpgradeManager: unhandled upgrade type {stat.statToUpgrade}");
                    break;
            }
        }
    }
    private void ApplyUpgrade_WeaponUnlock(WeaponUnlock weaponUnlock)
    {
        if (weaponUnlock.weaponPrefab == null)
        {
            Debug.Log("weaponPrefab is null.");
            return;
        }

        BaseWeapon newWeaponInstance = PlayerController.Instance.weaponManager.AddMiniWeapon(weaponUnlock.weaponPrefab);
        Debug.Log("Added weapon: " + weaponUnlock);

        UIManager.Instance.AddWeaponSlot(weaponUnlock, newWeaponInstance);
    }
    private void ApplyUpgrade_WeaponEvolution(WeaponEvolution weaponEvolution)
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

    public GameObject DisplayCard(BaseUpgradeData cardData)
    {
        // instantiate an upgrade.
        GameObject card = Instantiate(cardPrefab, cardContainer);
        UpgradeCardUI cardUI = card.GetComponent<UpgradeCardUI>();

        cardUI.Setup(cardData, OnCardSelected);

        return card;
    }

    public void DrawHand()
    {
        List<BaseUpgradeData> hand = Roller.BulkRoll(Acquired.acquiredUpgrades, handSize).ToList();
        foreach (BaseUpgradeData card in hand)
        {
            displayedCards.Add(DisplayCard(card));
        }
    }
    public void ClearHand()
    {
        foreach (var cardToRemove in displayedCards)
        {
            Destroy(cardToRemove);
        }
        displayedCards.Clear();
    }
    // ** SUB FUNCTIONS


    // TOOLS
    // external, getters/setters, non-method stuff (e.g., IEnumerator)
    private void IncrementUpgrade(BaseUpgradeData upgrade, bool debugLog = false)
    {
        // update our AcquiredUpgrades dictionary
        if (Acquired.acquiredUpgrades.ContainsKey(upgrade))
            Acquired.acquiredUpgrades[upgrade]++;
        else
            Acquired.acquiredUpgrades[upgrade] = 1;

        // print all acquired upgrades if debugLog
        if (debugLog)
        {
            foreach (KeyValuePair<BaseUpgradeData, int> upgradeEntry in Acquired.acquiredUpgrades)
                Debug.Log(upgradeEntry);
        }
    }

    // TOOLS


    // EVENTS & LISTENERS
    // put events and listeners here
    private void OnCardSelected(BaseUpgradeData upgrade)
    {
        ApplyUpgrade(upgrade);

        Roller.ApplyPity();

        EndUpgradeSelect();
    }
    // EVENTS & LISTENERS
}

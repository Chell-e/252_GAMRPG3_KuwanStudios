using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class UpgradeRoller : MonoBehaviour
{
    // singleton stuff
    public static UpgradeRoller Instance;
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
    public float pityIncrement;

    [Header("DEBUG")]
    [SerializeField] private List<float> pity = new List<float>();
    [SerializeField] private List<int> lastPityIndices = new List<int>(); // this 

    [Header("REFERENCES")]
    public UpgradePool upgradePool;

    // * DRIVER CODE
    private void Start()
    {
        for (int i = 0; i < upgradePool.tiers.Count; i++)
            pity.Add(0f);

        //StartUpgradeSelect();

        //Dictionary<BaseUpgradeData, int> fake = new Dictionary<BaseUpgradeData, int>();
        //Roll(3, fake);
        
    }

    public List<BaseUpgradeData> BulkRoll(Dictionary<BaseUpgradeData, int> acquiredUpgrades,
        int cardsToDraw)
    {
        List<BaseUpgradeData> upgradesToOffer = new List<BaseUpgradeData>();

        // first, make an array of lists for each tier.
        // DONT FORGET TO INITIALIZE IT!
        List<BaseUpgradeData>[] validArray = new List<BaseUpgradeData>[upgradePool.tiers.Count];
        for (int i = 0; i < validArray.Length; i++)
            validArray[i] = new List<BaseUpgradeData>();

        // then, roll list of tiers by indices
        List<int> drawnTiersByIndex = new List<int>();
        for (int i = 0; i < cardsToDraw; i++)
        {
            var targetTier = RollTier();
            int tierIndex = upgradePool.tiers.IndexOf(targetTier);
            drawnTiersByIndex.Add(tierIndex);
                        
            List<BaseUpgradeData> validPool = validArray[tierIndex];
            if (validPool.Count == 0)
            {
                // if array's current list is at count 0, overwrite the list with what we rolled
                validPool = GetAvailablePool(targetTier, acquiredUpgrades).ToList();
                validArray[tierIndex] = validPool;
            }

            var cardDrawn = DrawCardFromPool(validPool);

            // REMOVE ALL DUPES!
            if (cardDrawn.canDuplicate == false)
            {
                validPool.Remove(cardDrawn);
                validArray[tierIndex] = validPool;
            }

            upgradesToOffer.Add(cardDrawn);
            //DisplayCard(cardDrawn);
        }

        lastPityIndices = drawnTiersByIndex;
        //tiersByIndex = drawnTiersByIndex;

        return upgradesToOffer;
    }
    // * DRIVER CODE


    // *** CORE LOGIC
    public void ApplyPity(List<int> tiersByIndex = null)
    {
        if (tiersByIndex == null)
            tiersByIndex = lastPityIndices;

        tiersByIndex = tiersByIndex.Distinct().ToList();
        // this method makes tiersByIndex unique

        List<float> newPity = pity.ToList();

        for (int i = 0; i < upgradePool.tiers.Count; i++)
        {
            newPity[i] += pityIncrement;

            foreach (int index in tiersByIndex)
            {
                if (index == i)
                    newPity[i] = 0f;
            }
        }

        pity = newPity;
    }
    // *** CORE LOGIC


    // ** SUB FUNCTIONS
    public UpgradeTier RollTier()
    {
        float rarityRoll = Random.Range(0,
            upgradePool.tiers[upgradePool.tiers.Count - 1].rarityWeight);
        float cumulativeRange = 0;

        for (int i = 0; i < upgradePool.tiers.Count; i++)
        {
            UpgradeTier tier = upgradePool.tiers[i];
            cumulativeRange = tier.rarityWeight;
            float finalRange = cumulativeRange + pity[i];


            if (rarityRoll <= finalRange)
            {
                Debug.Log($"Rolled tier: {tier.rarityName}\nRoll was: {rarityRoll}/{cumulativeRange}[+{pity[i]}]");
                return tier;
            }
        }

        return upgradePool.tiers[upgradePool.tiers.Count - 1];
    }

    public List<BaseUpgradeData> GetAvailablePool(UpgradeTier sourceTier, Dictionary<BaseUpgradeData, int> acquiredUpgrades)
    {
        List<BaseUpgradeData> validPool = new List<BaseUpgradeData>();

        foreach (BaseUpgradeData upgrade in sourceTier.upgradePool)
        {

            int amountPicked = 0;
            acquiredUpgrades.TryGetValue(upgrade, out amountPicked);
            if (amountPicked >= upgrade.maxPicks && upgrade.maxPicks >= 0)
                continue; // skip if BEYOND MAX PICKS

            bool isValid = true;
            foreach (BaseUpgradeRequirement req in upgrade.requirements)
            {
                if (req.IsAvailable() == false)
                {
                    isValid = false;
                    break;
                }
            }
            if (isValid == false)
                continue; // skip if FAILED REQ


            validPool.Add(upgrade);
        }

        return validPool;
    }

    public BaseUpgradeData DrawCardFromPool(List<BaseUpgradeData> sourcePool)
    {
        int drawnIndex = Random.Range(0, sourcePool.Count);
        //Debug.Log($"drawnIndex: {drawnIndex}");
        BaseUpgradeData cardData = sourcePool[drawnIndex];

        return cardData;
    }
    // ** SUB FUNCTIONS


    // TOOLS
    // external, getters/setters, non-method stuff (e.g., IEnumerator)
    [ContextMenu("Debug.Log Pity")]
    private void DisplayPity()
    {
        string pityList = string.Empty;
        for (int i = 0; i < pity.Count; i++)
        {
            pityList += $"index {i}: {pity[i]}\n";
        }

        Debug.Log(pityList);
    }
    // TOOLS


    // EVENTS & LISTENERS
    // put events and listeners here

    // EVENTS & LISTENERS
}

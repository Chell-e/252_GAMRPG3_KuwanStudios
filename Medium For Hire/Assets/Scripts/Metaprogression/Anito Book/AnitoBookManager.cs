using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AnitoBookManager : MonoBehaviour
{
    // for SINGLETON
    public static AnitoBookManager Instance;
    private void Awake() 
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
    // for SINGLETON

    [Header("REFERENCES")]
    [SerializeField] private ChapterEntryUI entryUI;
    public ChapterEntry entryData;
    public PlayerData playerData;

    // * DRIVER CODE
    // mainly Start() and Update()
    private void Start()
    {
        if (PlayerData.Instance != null)
            playerData = PlayerData.Instance;
    }

    public void InitializeEntryData(ChapterEntry _entryData)
    {
        entryData = _entryData;
        
        entryUI.CloseStoryPage();

        if (entryData.chapterUnlocked == true)
        {
            LoadEntryData();
        }
        else 
        {
            entryUI.illusLock.enabled = true;
            ClearPage(entryData);
        }

        RefreshStoryButton();

        DoReqLogic();
    }
    // * DRIVER CODE


    // *** CORE LOGIC
    // these are functions that coordinate smaller functions below
    public void DoReqLogic()
    {
        if (entryData == null) return;

        if (CheckRequirement_Illus() && CheckRequirement_Name() && CheckRequirement_Desc() 
            && entryData.chapterUnlocked == false)
        {
            entryUI.EnableHoveredPrimaryUnlockable();
        }
        else
        {
            entryUI.DisableHoveredPrimaryUnlockable();
        }

        if(entryData.chapterUnlocked == true && CheckRequirement_TornPages() && !entryData.storyUnlocked)
        {
            entryUI.EnableHoveredSecondaryUnlockable();
        }
        else
        {
            entryUI.DisableHoveredSecondaryUnlockable();
        }
    }

    public void LoadEntryData()
    {
        if (entryData == null) return;

        if (!CheckRequirement_Illus() && !CheckRequirement_Name() && !CheckRequirement_Desc()
            && entryData.chapterUnlocked == false)
        {
            return;
        }

        entryUI.LoadIllustration(entryData);
        entryUI.LoadName(entryData);
        entryUI.LoadDescription(entryData);

        entryUI.DisableHoveredPrimaryUnlockable();
        entryUI.illusLock.enabled = false;

        entryData.chapterUnlocked = true;
    }

    public void UnlockStoryButton()
    {
        if (entryData == null) return;

        if (!entryData.chapterUnlocked || !CheckRequirement_TornPages()) return;
    
        entryData.storyUnlocked = true;

        RefreshStoryButton();
        DoReqLogic();

        //entryUI.DisableHoveredSecondaryUnlockable();
        //entryUI.storyLock.enabled = false;
        //entryUI.storyButton.SetActive(true);
    }

    public void RefreshStoryButton()
    {
        ////entryUI.DisableHoveredSecondaryUnlockable();

        //if (entryData.storyUnlocked == false && CheckRequirement_TornPages() == false)
        //{
        //    entryUI.DisableHoveredSecondaryUnlockable();
        //}
        //if (entryData.storyUnlocked == true && CheckRequirement_TornPages())
        //{
        //    entryUI.EnableHoveredSecondaryUnlockable();
        //}

        if (entryData == null) return;

        if (entryData.storyUnlocked)
        {
            entryUI.storyButton.SetActive(true);
            entryUI.storyLock.enabled = false;
        }
        else
        {
            entryUI.storyButton.SetActive(false);
            entryUI.storyLock.enabled = true;
        }
    }

    public void LoadStoryData()
    {
        if (entryData == null) return;

        if (entryData.storyUnlocked)
        {
            entryUI.DisplayStoryPage(entryData);
        }
    }
    // *** CORE LOGIC


    // ** SUB FUNCTIONS
    // more "individual" functions
    private bool CheckRequirement_Illus()
    {
        return playerData.GetEnemyKillData(entryData.chapterName).killAmount
            >= entryData.killsNeededToUnlockImage;
    }

    private bool CheckRequirement_Name()
    {
        return playerData.GetEnemyKillData(entryData.chapterName).killAmount
            >= entryData.killsNeededToUnlockName;
    }   

    private bool CheckRequirement_Desc()
    {
        return playerData.GetEnemyKillData(entryData.chapterName).killAmount
            >= entryData.killsNeededToUnlockDesc;
    }

    private bool CheckRequirement_TornPages()
    {
        return playerData.tornPagesAmount >= 6;
    }

    private void ClearPage(ChapterEntry _entryData)
    {
        entryUI.UnloadIllustration(_entryData);
        entryUI.UnloadName(_entryData);
        entryUI.UnloadDescription(_entryData);
    }
    // ** SUB FUNCTIONS


    // TOOLS
    // external, getters/setters, non-method stuff (e.g., IEnumerator)
    public bool IsChapterUnlocked(ChapterEntry entry)
    {
        return PlayerData.Instance.GetTotalKills(entry.chapterName) >= 1;
    }
    // TOOLS


    // EVENTS & LISTENERS
    // put events and listeners here

    // EVENTS & LISTENERS

}

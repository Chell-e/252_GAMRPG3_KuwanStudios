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

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Escape))
        //{
        //    OnBackPressed();
        //}
    }

    public void InitializeEntryData(ChapterEntry _entryData)
    {
        entryData = _entryData;

        if (entryData.chapterUnlocked == true)
        {
            LoadEntryData();
        }
        else 
        {
            entryUI.illusLock.enabled = true;
            ClearPage(entryData);
        }

        DoReqLogic();
    }
    // * DRIVER CODE


    // *** CORE LOGIC
    // these are functions that coordinate smaller functions below
    public void DoReqLogic()
    {
        if (entryData == null) return;

        //if (CheckRequirement_Illus()) entryUI.LoadIllustration(entryData);
        //if (CheckRequirement_Name()) entryUI.LoadName(entryData);
        //if (CheckRequirement_Desc()) entryUI.LoadDescription(entryData);

        if (CheckRequirement_Illus() && CheckRequirement_Name() && CheckRequirement_Desc() 
            && entryData.chapterUnlocked == false)
        {
            entryUI.EnableHoveredPrimaryUnlockable();
        }
        else
        {
            entryUI.DisableHoveredPrimaryUnlockable();
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

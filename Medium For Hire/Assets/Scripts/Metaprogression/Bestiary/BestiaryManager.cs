using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BestiaryManager : MonoBehaviour
{
    [Header("Bestiary UI")]
    [SerializeField] private GameObject creatureListUI;
    [SerializeField] private GameObject creaturePageUI;

    public static BestiaryManager Instance;

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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OnBackPressed();
        }
    }

    public void OnBackPressed()
    {
        if (creaturePageUI.activeSelf)
        {
            ShowList();
        }
        else
        {
            GameSceneManager.Instance.LoadPreviousScene();
        }
    }

    public void ShowList()
    {
        creaturePageUI.SetActive(false);
        creatureListUI.SetActive(true);
    }

    public void ShowDetailedPage(PageEntry entry)
    {
        creatureListUI.SetActive(false);
        creaturePageUI.SetActive(true);

        creaturePageUI.GetComponent<CreaturePageDisplay>().SetCreaturePage(entry);
    }

    // check if name can be unlocked
    public bool IsNameUnlocked(PageEntry entry)
    {
        return PlayerData.Instance.GetTotalKills(entry.entryName) >= entry.killsNeededToUnlockName;
    }

    // check if image can be unlocked
    public bool IsImageUnlocked(PageEntry entry)
    {
        return PlayerData.Instance.GetTotalKills(entry.entryName) >= entry.killsNeededToUnlockImage;
    }

    // check if lore can be unlocked
    public bool IsLoreUnlocked(PageEntry entry)
    {
        return PlayerData.Instance.GetTotalKills(entry.entryName) >= entry.killsNeededToUnlockLore;
    }

    // overall percentage progress
    public float GetUnlockProgress(PageEntry entry)
    {
        int currentKills = PlayerData.Instance.GetTotalKills(entry.entryName);
        int maxNeeded = entry.MaxKillsNeeded;

        return Mathf.Clamp01((float)currentKills / maxNeeded) * 100f;
    }
}

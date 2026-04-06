using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelSelectManager : MonoBehaviour
{
    [Header("Levels")]
    public LevelData[] availableLevels; // if we want to create more levels
    public LevelSelectUI[] levelSelections;

    //    [Header("UI References")]
    ////public Image mapPreview;
    //public TextMeshProUGUI levelNameText;
    ////public TextMeshProUGUI levelDescription; 
    //public TextMeshProUGUI enemiesText; // normal enemies
    //public TextMeshProUGUI bossText;
    //public TextMeshProUGUI superstitionText;
    //public TextMeshProUGUI rewardsText;
    //public TextMeshProUGUI mapText;

    //private LevelData currentLevel; // level selected

    private void Start()
    {
        if (availableLevels.Length == 0)
        {
            Debug.Log("No levels.");
            return;
        }

        RefreshLevels();
    }

    public void RefreshLevels()
    {
        // to prevent duplicates, shuffle the list
        List<LevelData> shuffledLevels = new List<LevelData>(availableLevels);

        for (int i = 0; i < shuffledLevels.Count; i++)
        {
            LevelData temp = shuffledLevels[i];

            int randomIndex = Random.Range(i, shuffledLevels.Count);
            shuffledLevels[i] = shuffledLevels[randomIndex];
            shuffledLevels[randomIndex] = temp;
        }

        // assign 1 unique level to each slot
        for (int i = 0; i < levelSelections.Length; i++)
        {
            if (i < shuffledLevels.Count)
            {
                levelSelections[i].gameObject.SetActive(true);
                levelSelections[i].SetUp(shuffledLevels[i], this);
            }
            else
            {
                levelSelections[i].gameObject.SetActive(false);
            }
        }

        //int randomIndex = Random.Range(0, availableLevels.Length);
        //currentLevel = availableLevels[randomIndex];

        //UpdateLevelUI();
    }

    public string GetEnemyListString(EnemyPoolData enemyPool)
    {
        if (enemyPool == null || enemyPool.enemyPool == null || enemyPool.enemyPool.Count == 0)
            return "None";

        string enemyList = "";
        for (int i = 0; i < enemyPool.enemyPool.Count; i++)
        {
            enemyList += enemyPool.enemyPool[i].enemyName;

            if (i < enemyPool.enemyPool.Count - 1)
                enemyList += ", ";
        }

        return enemyList;
    }

    public void StartGame(LevelData currentLevel, int currentLevelRewards)
    {
        if (currentLevel == null)
        {
            Debug.Log("No level selected.");
            return;
        }

        StageManager.CurrentLevel = currentLevel;
        StageManager.CurrentLevelRewards = currentLevelRewards;

        SceneManager.LoadScene("GameScene");
    }
}
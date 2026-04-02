using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LevelSelectUI : MonoBehaviour
{
        [Header("UI References")]
    public Image mapPreview;
    public TextMeshProUGUI levelNameText;
    //public TextMeshProUGUI levelDescription; 
    public TextMeshProUGUI enemiesText; // normal enemies
    public TextMeshProUGUI bossText;
    public TextMeshProUGUI superstitionText;
    public TextMeshProUGUI rewardsText;
    public TextMeshProUGUI mapText;

    private LevelData levelData;
    private LevelSelectManager levelSelectManager;

    public void SetUp(LevelData data, LevelSelectManager manager)
    {
        levelData = data;
        levelSelectManager = manager;

        levelNameText.text = data.levelName;
        //levelDescription.text = currentLevel.description;
        enemiesText.text = manager.GetEnemyListString(data.normalEnemies);
        bossText.text = data.boss.bossName;
        superstitionText.text = data.superstition.name;
        rewardsText.text = data.endRewards.rewardAmount.ToString();
        mapText.text = data.map.name;
    }

    public void OnClickSelect()
    {
        levelSelectManager.StartGame(levelData);
    }
}

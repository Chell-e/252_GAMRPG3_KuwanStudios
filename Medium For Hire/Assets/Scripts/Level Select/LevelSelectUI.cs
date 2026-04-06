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
    public TextMeshProUGUI enemiesText; // normal enemies
    public TextMeshProUGUI bossText;
    public TextMeshProUGUI superstitionText;
    public TextMeshProUGUI rewardsText;
    public TextMeshProUGUI mapText;

    private LevelData levelData;
    private LevelSelectManager levelSelectManager;

    private int levelRewards;

    public void SetUp(LevelData data, LevelSelectManager manager)
    {
        levelData = data;
        levelSelectManager = manager;

        levelNameText.text = data.levelName;
        //levelDescription.text = currentLevel.description;
        enemiesText.text = manager.GetEnemyListString(data.normalEnemies);
        bossText.text = data.boss.bossName;
        superstitionText.text = data.superstition.superstitionName;

        levelRewards = data.endRewards.GetRandomAmount();
        rewardsText.text = levelRewards.ToString();

        mapText.text = data.map.name;

        // random map preview
        if (mapPreview != null && data.map != null && data.map.mapSprites != null)
        {
            if (data.map.mapSprites.Length > 0)
            {
                int randomIndex = Random.Range(0, data.map.mapSprites.Length);
                mapPreview.sprite = data.map.mapSprites[randomIndex];
            }
        }
    }

    public void OnClickSelect()
    {
        levelSelectManager.StartGame(levelData, levelRewards);
    }
}
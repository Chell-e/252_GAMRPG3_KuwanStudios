using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveDataJSON : MonoBehaviour
{
    private PlayerData playerData;
    private EnemyKillData enemyKillData;

    public static SaveDataJSON Instance;

    private void Awake()
    {
        // singleton 
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }

        DontDestroyOnLoad(this.gameObject);

        playerData = PlayerData.Instance; // new
        string filePath = Path.Combine(Application.persistentDataPath + "PlayerData.json");

        LoadData();
    }

    private void Start()
    {
        playerData = PlayerData.Instance;
    }

    public void SaveData()
    {
        string json = JsonUtility.ToJson(playerData);
        string filePath = Path.Combine(Application.persistentDataPath + "PlayerData.json");

        //Debug.Log(json);
        //Debug.Log(filePath);

        System.IO.File.WriteAllText(filePath, json);
        Debug.Log("Data saved.");
    }

    public void LoadData()
    {
        string filePath = Path.Combine(Application.persistentDataPath + "PlayerData.json");

        if (File.Exists(filePath))
        {
            string json = System.IO.File.ReadAllText(filePath);

            PlayerData loadedData = JsonUtility.FromJson<PlayerData>(json);

            // dont't forget to update singleton
            //PlayerData.SetInstance(loadedData);
            playerData.SetPlayerData(loadedData.pilonAmount, loadedData.enemyKills, loadedData.healthLevel, loadedData.attackSpeedLevel, loadedData.moveSpeedLevel, loadedData.projectileSpeedLevel, loadedData.projectileSpeedLevel, loadedData.pickupRangeLevel);

            Debug.Log("Data loaded");
        }
        else
        {
            Debug.Log("No saved file found.");
        }
    }

}
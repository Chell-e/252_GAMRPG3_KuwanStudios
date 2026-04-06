using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    // ===== PERMANENT UPGRADE LEVELS (0-5) =====
    public int healthLevel = 0;
    public int damageLevel = 0;
    public int attackSpeedLevel = 0;
    public int moveSpeedLevel = 0;
    public int projectileSpeedLevel = 0;
    public int pickupRangeLevel = 0;

    // ===== PILON =====
    public int pilonAmount;


    // ===== UNLOCKED ITEMS =====


    // ===== ENEMIES KILLED =====
    public List<EnemyKillData> enemyKills = new List<EnemyKillData>();


    // *EVENTS
    public delegate void OnPlayerDataChange(int pilonAmount, int healthLevel, int damageLevel, int attackSpeedLevel, int moveSpeedLevel, int projectileSpeedLevel, int pickupRangeLevel);
    public static event OnPlayerDataChange onDataChange;

    private static PlayerData _instance;

    public static PlayerData Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new PlayerData(0, 0, 0, 0, 0, 0, 0);
            }

            return _instance;
        }
    }

    public static void SetInstance(PlayerData data)
    {
        _instance = data;
    }

    // add more later here (stats to permanently upgrade)
    private PlayerData(int _pilonAmount, int _healthLevel, int _damageLevel, int _attackSpeedLevel, int _moveSpeedLevel, int _projectileSpeedLevel, int _pickupRangeLevel)
    {
        this.pilonAmount = _pilonAmount;
        this.healthLevel = _healthLevel;
        this.damageLevel = _damageLevel;
        this.attackSpeedLevel = _attackSpeedLevel;
        this.moveSpeedLevel = _moveSpeedLevel;
        this.projectileSpeedLevel = _projectileSpeedLevel;
        this.pickupRangeLevel = _pickupRangeLevel;
    }

    public void SetPlayerData(int _pilonAmount, List<EnemyKillData> _enemyKills, int _healthLevel, int _damageLevel, int _attackSpeedLevel, int _moveSpeedLevel, int _projectileSpeedLevel, int _pickupRangeLevel)
    {
        this.pilonAmount = _pilonAmount;
        this.enemyKills = _enemyKills;

        this.healthLevel = _healthLevel;
        this.damageLevel = _damageLevel;
        this.attackSpeedLevel = _attackSpeedLevel;
        this.moveSpeedLevel = _moveSpeedLevel;
        this.projectileSpeedLevel = _projectileSpeedLevel;
        this.pickupRangeLevel = _pickupRangeLevel;

        onDataChange?.Invoke(_pilonAmount, _healthLevel, _damageLevel, _attackSpeedLevel, _moveSpeedLevel, _projectileSpeedLevel, _pickupRangeLevel);
    }

    public EnemyKillData GetEnemyKillData(string name)
    {
        foreach (EnemyKillData data in enemyKills)
        {
            if (data.enemyName == name)
                return data;
        }

        // if not found, create one
        EnemyKillData newEnemy = new EnemyKillData(name);
        enemyKills.Add(newEnemy);
        return newEnemy;
    }

    public int GetTotalKills(string name)
    {
        // add all kills
        var data = GetEnemyKillData(name);

        return data.normalKills + data.eliteKills + data.bossKills;
    }

    public void AddPilon(int amount)
    {
        pilonAmount += amount;
        onDataChange?.Invoke(pilonAmount, healthLevel, damageLevel, attackSpeedLevel, moveSpeedLevel, projectileSpeedLevel, pickupRangeLevel);

        Debug.Log(amount + " pilon is added. Current pilon now is: " + pilonAmount);
    }
}

[System.Serializable]
public class EnemyKillData
{
    public string enemyName;
    public int normalKills;
    public int eliteKills;
    public int bossKills;

    public EnemyKillData(string name)
    {
        enemyName = name;
        normalKills = 0;
        eliteKills = 0;
        bossKills = 0;
    }
}
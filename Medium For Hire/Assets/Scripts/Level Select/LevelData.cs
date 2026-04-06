using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Levels/Level Data")]
public class LevelData : ScriptableObject
{
    [Header("Basic Info")]
    public string levelName; // job title
    public string description; // brief job description

    [Header("Stage Modules")]
    public MapData map;
    public EnemyPoolData normalEnemies;
    public EnemyPoolData eliteEnemies;
    public BossData boss;
    public SuperstitionData superstition;
    public RewardData endRewards;
}

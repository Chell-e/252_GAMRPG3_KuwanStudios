using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Levels/Enemy Pool Data")]
public class EnemyPoolData : ScriptableObject
{
    public List<EnemyData> enemyPool = new List<EnemyData>();
}

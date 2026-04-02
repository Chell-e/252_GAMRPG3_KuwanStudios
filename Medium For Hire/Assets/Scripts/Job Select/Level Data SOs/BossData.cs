using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Levels/Boss Data")]
public class BossData : ScriptableObject
{
    public string bossName;
    public GameObject bossPrefab;
}

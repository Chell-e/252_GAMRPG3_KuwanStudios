using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Levels/Reward Data")]
public class RewardData : ScriptableObject
{
    public int rewardAmount;
    public GameObject rewardPrefab;
}

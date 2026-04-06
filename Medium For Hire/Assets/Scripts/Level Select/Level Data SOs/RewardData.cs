using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Levels/Reward Data")]
public class RewardData : ScriptableObject
{
    public int minRewardAmount;
    public int maxRewardAmount;

    public GameObject rewardPrefab;

    public int GetRandomAmount()
    {
        return Random.Range(minRewardAmount, maxRewardAmount + 1);
    }
}
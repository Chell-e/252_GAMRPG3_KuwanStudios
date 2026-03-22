using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DropItem : ScriptableObject
{
    public GameObject itemPrefab;
    [Range(0f, 1f)] public float dropChance;
}

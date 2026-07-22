using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DropItem : ScriptableObject
{
    public GameObject itemPrefab;
    public float dropChance;
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "Levels/Map Data")]
public class MapData : ScriptableObject
{
    public string mapName;
    public Sprite[] mapSprites;
    public GameObject mapPrefab;
}

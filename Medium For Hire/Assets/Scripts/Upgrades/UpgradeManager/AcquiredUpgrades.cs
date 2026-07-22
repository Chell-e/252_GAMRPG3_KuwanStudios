using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class AcquiredUpgrades : MonoBehaviour, ISerializationCallbackReceiver
{
    public Dictionary<BaseUpgradeData, int> acquiredUpgrades = new Dictionary<BaseUpgradeData, int>();

    // *** BELOW: exclusively for editor viewing

    [SerializeField] private List<string> cards = new List<string>();
    [SerializeField] private List<int> picks = new List<int>();
    public void OnBeforeSerialize()
    {
        cards.Clear();
        picks.Clear();
        foreach (var kvp in acquiredUpgrades)
        {
            cards.Add(kvp.Key.title);
            picks.Add(kvp.Value);
        }
    }
    public void OnAfterDeserialize() { }
}

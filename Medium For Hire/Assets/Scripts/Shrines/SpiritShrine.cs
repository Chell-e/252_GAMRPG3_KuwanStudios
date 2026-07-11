using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiritShrine : BaseShrine
{
    [Header("Superstitions Pool")]
    [SerializeField] private List<SuperstitionData> possibleSuperstitions;
    private SuperstitionData rolledSuperstition;

    [SerializeField] private float nextSpawnDelay = 30f;

    public override void Interact()
    {
       if (possibleSuperstitions == null || possibleSuperstitions.Count == 0)
       {
            Debug.Log("No superstitions");
            return;
       }

       // roll a random superstition
       int randomIndex = Random.Range(0, possibleSuperstitions.Count);
        rolledSuperstition = possibleSuperstitions[randomIndex];

        ShrineUIManager.Instance.OpenSuperstitionPanel(this, shrineName, rolledSuperstition);
    }

    public void ExecuteAccept()
    {
        if (rolledSuperstition != null)
        {
            SuperstitionManager.Instance.ActivateSuperstition(rolledSuperstition);
        }

        RelocateSpawn();
    }

    public void ExecuteDecline()
    {
        RelocateSpawn();
    }

    private void RelocateSpawn()
    {
        rolledSuperstition = null;
        DespawnShrine();
        ShrineSpawner.Instance.RequestDelayedRespawn(this, nextSpawnDelay);
    }
}
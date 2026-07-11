//using System.Collections;
//using System.Collections.Generic;
//using Unity.VisualScripting;
//using UnityEngine;

//public class ApolakiShrine : BaseShrine
//{
//    [SerializeField] private float nextSpawnDelay = 300f;

//    public override void Interact()
//    {
//        ShrineUIManager.Instance.OpenNormalPanel(this, shrineName, shrineFlavorText, shrineDescription);
//    }

//    public void ExecuteAccept()
//    {
//        StartChallenge();
//        DespawnShrine();
//    }

//    public void ExecuteDecline()
//    {
//        Debug.Log("Apolaki Shrine declined. Staying here...");
//    }

//    private void StartChallenge()
//    {
//        Debug.Log("APOLAKI SHRINE ACTIVATED: STARTING CHALLENGE...!");
//        // Implement challenge logic here
//    }

//    public void OnChallengeComplete()
//    {
//        // reward player here
//    }
//}
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShrinePointer : MonoBehaviour
{
    [SerializeField] private GameObject arrow; 

    void Update()
    {
        Vector3 nearestSpiritShrinePos = GetNearestSpiritShrine();

        if (SuperstitionManager.Instance.sitanCorruptionActive && nearestSpiritShrinePos != Vector3.zero)
        {
            arrow.SetActive(true);
            Vector3 pointingDirection = nearestSpiritShrinePos - PlayerController.Instance.transform.position;
            this.transform.right = pointingDirection.normalized;
        }
        else
        {
            arrow.SetActive(false);
        }
    }

    private Vector3 GetNearestSpiritShrine()
    {
        float minimumDistance = 999f;
        Vector3 nearestPosition = new Vector3();

        foreach (var shrine in ShrineSpawner.Instance.activeShrines)
        {
            if (shrine != null && shrine.CurrentType == ShrineType.Spirit)
            {
                float dist = Vector3.Distance(PlayerController.Instance.transform.position,
                                shrine.transform.position);

                if (dist < minimumDistance)
                {
                    minimumDistance = dist;
                    nearestPosition = shrine.transform.position;

                }
            }


        }
        return nearestPosition;
    }
}

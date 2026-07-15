using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGhostSprite : MonoBehaviour
{
    [SerializeField] private float ghostDelay;
    private float ghostDelaySeconds;
    [SerializeField] public GameObject ghost;
    public bool makeGhost = false;

    private void Start()
    {
        ghostDelaySeconds = ghostDelay;
    }

    private void Update()
    {
        if (makeGhost)
        {
            if (ghostDelaySeconds > 0)
            {
                ghostDelaySeconds -= Time.deltaTime;
            }
            else
            {
                GameObject currGhost = Instantiate(ghost, transform.position, transform.rotation);
                ghostDelaySeconds = ghostDelay;
            }
        }
    }
}

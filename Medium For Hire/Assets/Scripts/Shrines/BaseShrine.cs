using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseShrine : MonoBehaviour
{
    [SerializeField] protected string shrineName;
    [TextArea]
    [SerializeField] protected string shrineFlavorText;
    [SerializeField] protected string shrineDescription;
    [SerializeField] protected GameObject interactionPromptUI;

    public Transform currentSpawnPoint;
    protected SpriteRenderer spriteRenderer;
    protected Animator shrineAnimator;

    protected void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        shrineAnimator = GetComponent<Animator>();
    }

    protected void Start()
    {
        interactionPromptUI.SetActive(false);
    }

    public virtual void Interact() { }

    protected void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerController player = collision.gameObject.GetComponent<PlayerController>();

        if (player != null)
        {
            // tell player which shrine they're interacting w
            player.SetCurrentShrine(this);

            // show interaction prompt UI
            interactionPromptUI.SetActive(true);
        }
    }

    protected void OnTriggerExit2D(Collider2D collision)
    {
        PlayerController player = collision.gameObject.GetComponent<PlayerController>();

        if (player != null)
        {
            // tell player they left the shrine
            player.ClearCurrentShrine(this);

            // hide interaction prompt UI
            interactionPromptUI.SetActive(false);
        }
    }

    public void DespawnShrine()
    {
        Debug.Log("Despawned shrine: " + shrineName);

        if (currentSpawnPoint != null & ShrineSpawner.Instance != null)
        {
            // free the current location
            ShrineSpawner.Instance.FreeSpawnPoint(currentSpawnPoint);
        }

        if (shrineAnimator != null)
        {
            // play disappearing animation
            //shrineAnimator.SetBool("IsDisappearing", true);
            
        }

        gameObject.SetActive(false);
    }
}
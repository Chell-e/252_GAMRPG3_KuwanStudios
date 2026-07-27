using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ShrineType
{
    Empty,
    Spirit,
    Akasi,
    Apolaki
}

public class BaseShrine : MonoBehaviour
{
    public static BaseShrine Instance;

    [SerializeField] private ShrineType currentType = ShrineType.Empty;

    [SerializeField] private GameObject interactionPromptUI;
    [SerializeField] private Sprite emptyShrineSprite;

    public float healAmount = 5f;

    [Header("Spirit Shrine")]
    [SerializeField] private string spiritName = "The Shrine of Spirits";
    [SerializeField] private Sprite spiritSprite;
    [SerializeField] private string spiritIdleAnim = "Spirit_Idle";
    [SerializeField] private string spiritDisappearAnim = "Spirit_Disappear";
    [SerializeField] private List<SuperstitionData> possibleSuperstitions;
    [SerializeField] private SuperstitionData rolledSuperstition; 

        [Header("Akasi Shrine")]
    [SerializeField] private string akasiName = "The Shrine of Akasi";
    [TextArea] 
    [SerializeField] private string akasiFlavorText;
    [SerializeField] private string akasiDescription;
    [SerializeField] private Sprite akasiSprite;
    [SerializeField] private string akasiIdleAnim = "Akasi_Idle";
    [SerializeField] private string akasiDisappearAnim = "Akasi_Disappear";

    [Header("Apolaki Shrine")]
    [SerializeField] private string apolakiName = "The Shrine of Apolaki";
    [TextArea]
    [SerializeField] private string apolakiFlavorText;
    [SerializeField] private string apolakiDescription;
    [SerializeField] private Sprite apolakiSprite;
    [SerializeField] private string apolakiIdleAnim = "Apolaki_Idle";
    [SerializeField] private string apolakiDisappearAnim = "Apolaki_Disappear";
    private int elitesToBeKilled = 3;
    private int elitesKilled = 0;
    private bool isApolakiShrineActive = false;
    List<BaseEnemy> targets = new List<BaseEnemy>();

    private string shrineName;
    private string shrineFlavorText;
    private string shrineDescription;
    private string disappearAnimName;

    private SpriteRenderer spriteRenderer;
    private Animator shrineAnim;

    private bool isPlayerInside = false; // for triggering interact prompt
    private bool isAnimating = false; 

    public ShrineType CurrentType => currentType;

    protected void Awake()
    {
        // singleton 
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        shrineAnim = GetComponentInChildren<Animator>();
    }

    protected void Start()
    {
        interactionPromptUI.SetActive(false);
        SetShrineType(ShrineType.Empty);
    }

    private void Update()
    {
        if (isApolakiShrineActive)
        {
            CheckChallengeCompleted();
        }
    }

    public void SetShrineType(ShrineType newType)
    {
        currentType = newType;
        interactionPromptUI.SetActive(false);

        isAnimating = false;

        switch (currentType)
        {
            case ShrineType.Empty:
                shrineName = "";
                disappearAnimName = "";
                spriteRenderer.sprite = emptyShrineSprite;
                break;
            case ShrineType.Spirit:
                shrineName = spiritName;
                spriteRenderer.sprite = spiritSprite;
                disappearAnimName = spiritDisappearAnim;
                if (shrineAnim != null && !string.IsNullOrEmpty(spiritIdleAnim)) shrineAnim.Play(spiritIdleAnim);
                break;
            case ShrineType.Akasi:
                shrineName = akasiName;
                shrineFlavorText = akasiFlavorText;
                shrineDescription = akasiDescription;
                spriteRenderer.sprite = akasiSprite;
                disappearAnimName = akasiDisappearAnim;
                if (shrineAnim != null && !string.IsNullOrEmpty(akasiIdleAnim)) shrineAnim.Play(akasiIdleAnim);
                break;
            case ShrineType.Apolaki:
                shrineName = apolakiName;
                shrineFlavorText = apolakiFlavorText;
                shrineDescription = apolakiDescription;
                spriteRenderer.sprite = apolakiSprite;
                disappearAnimName = apolakiDisappearAnim;
                if (shrineAnim != null && !string.IsNullOrEmpty(apolakiIdleAnim)) shrineAnim.Play(apolakiIdleAnim);
                break;
        }

        if (isPlayerInside && currentType != ShrineType.Empty)
        {
            interactionPromptUI.SetActive(true);
        }
    }

    public void Interact()
    {
        if (currentType == ShrineType.Empty || isAnimating) return;

        if (currentType == ShrineType.Spirit)
        {
            if (possibleSuperstitions == null || possibleSuperstitions.Count == 0) return;

            // start rolling a random superstition
            rolledSuperstition = possibleSuperstitions[Random.Range(0, possibleSuperstitions.Count)];
            if (rolledSuperstition != null)
            {
                ShrineUIManager.Instance.OpenSuperstitionPanel(this, spiritName, rolledSuperstition);
            }
        }
        else
        {
            ShrineUIManager.Instance.OpenNormalPanel(this, shrineName, shrineFlavorText, shrineDescription);
        }
    }

    public void ExecuteAccept()
    {
        if (currentType == ShrineType.Empty || isAnimating) return;

        if (currentType == ShrineType.Spirit)
        {
            // superstition logic here
            
            SuperstitionManager.Instance.ActivateSuperstition(rolledSuperstition);
        }
        else if (currentType == ShrineType.Akasi)
        {
            // heal logic here
            HealthComponent playerHealth = PlayerController.Instance.GetComponent<HealthComponent>();

            if (playerHealth != null)
            {
                playerHealth.HealToFull();
                UIManager.Instance.UpdateHpUI();
            }

        }
        else if (currentType == ShrineType.Apolaki)
        {
            // spawn elites (marked)
            isApolakiShrineActive = true;

            targets = PoolSpawner.Instance.SpawnChallengeEliteEnemies(
                PlayerController.Instance.GetComponent<PlayerStats>().currentLevel);

            foreach (BaseEnemy target in targets)
            {
                target.OnAfterEnemyDeath += HandleTargetDeath;
            }
        }

        DespawnShrine();
        ShrineHealsPlayer();
    }

    public void HandleTargetDeath()
    {
        elitesKilled++;
        //Debug.Log("target killed");
    }

    private void CheckChallengeCompleted()
    {
        if (elitesKilled >= elitesToBeKilled)
        {
            // win
            isApolakiShrineActive = false;
            
            // unsubscribe
            foreach (BaseEnemy target in targets)
            {
                target.OnAfterEnemyDeath -= HandleTargetDeath;
            }

            // reward 1 mini-weapon

        }
    }

    public void ExecuteDecline()
    {
        // akasi & apolaki stays on map til accepted
        if (currentType == ShrineType.Spirit)
        {
            DespawnShrine();
        }
    }

    public void DespawnShrine()
    {
        if (shrineAnim != null && !string.IsNullOrEmpty(disappearAnimName))
        {
            StartCoroutine(PlayDisappear());
        }
        else
        {
            VacateSpot();
        }
    }

    private void ShrineHealsPlayer()
    {
        if (PlayerController.Instance != null)
        {
            PlayerController.Instance.GetComponent<HealthComponent>().Heal(healAmount);
            //Debug.Log("player healed by shrine");
        }
    }

    private IEnumerator PlayDisappear()
    {
        isAnimating = true;
        interactionPromptUI.SetActive(false);

        shrineAnim.Play(disappearAnimName);

        float animLength = 0.5f;
        if (shrineAnim != null)
        {
            RuntimeAnimatorController ac = shrineAnim.runtimeAnimatorController;
            foreach (AnimationClip clip in ac.animationClips)
            {
                if (clip.name == disappearAnimName)
                {
                    animLength = clip.length;
                    break;
                }    
            }
        }

        yield return new WaitForSeconds(animLength);
        VacateSpot();
    }

    public void VacateSpot()
    {
        ShrineType previousType = currentType;
        SetShrineType(ShrineType.Empty);

        if (ShrineSpawner.Instance != null)
        {
            ShrineSpawner.Instance.FreeActiveShrineSpot(this, previousType);
        }
    }

    protected void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerController player = collision.gameObject.GetComponent<PlayerController>();
        if (player != null)
        {
            isPlayerInside = true;
            player.SetCurrentShrine(this);

            // no interaction prompt if u have a superstition
            if (currentType != ShrineType.Empty)
            {
                if (currentType == ShrineType.Spirit && SuperstitionManager.Instance.hasSuperstition) return;

                interactionPromptUI.SetActive(true);
            }
        }
    }

    protected void OnTriggerExit2D(Collider2D collision)
    {
        PlayerController player = collision.gameObject.GetComponent<PlayerController>();
        if (player != null)
        {
            isPlayerInside = false;
            player.ClearCurrentShrine(this);
            interactionPromptUI.SetActive(false);
        }
    }
}
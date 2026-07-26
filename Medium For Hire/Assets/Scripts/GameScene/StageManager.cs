using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// IN CHARGE OF THE CURRENT STAGE/LEVEL
public class StageManager : MonoBehaviour
{
    [SerializeField] public PlayerEvents Events;
    [SerializeField] private PoolSpawner poolSpawner;

    [SerializeField] private Timer timer;

    [SerializeField] private GameObject mapPrefab;

    public static StageManager Instance;
    public bool isGameOver = false;
    public int tornPagesCollected = 0;

    public static WeaponData SelectedWeapon { get; set; }

    public static bool IsGameOver { get; private set; }

    // temp storage for the current run
    private Dictionary<string, int> runKills = new Dictionary<string, int>();

    private void Awake() // for SINGLETON
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
    }

    private void Start()
    {
        AssembleStage();

        if (SoundManager.Instance == null) return;
        SoundManager.Instance.PlayBGM(0, true);
    }

    private void Update()
    {

    }


    private void OnEnable()
    {
        isGameOver = false;
        Time.timeScale = 1f;

        Events.OnPlayerDeath += CompleteLevel;
        OnDeath.OnBossDeath += CompleteLevel;
        //SuperstitionManager.OnSuperstitionBroken += CheckSuperstition;

    }

    private void OnDisable()
    {
        if (SoundManager.Instance != null )
        {
            SoundManager.Instance.StopBGM();
        }

        Events.OnPlayerDeath -= CompleteLevel;
        OnDeath.OnBossDeath -= CompleteLevel;
        //SuperstitionManager.OnSuperstitionBroken -= CheckSuperstition;

    }

    private void AssembleStage()
    {
        if (mapPrefab != null)
        {
            Instantiate(mapPrefab, Vector3.zero, Quaternion.identity);
        }

        if (SelectedWeapon != null)
        {
            WeaponManager.Instance.EquipMainWeapon(SelectedWeapon);
        }
    }

    public void RegisterKill(string name)
    {
        if (!runKills.ContainsKey(name))
            runKills[name] = 1;
        else
            runKills[name] += 1;
    }

    public void RegisterTornPages()
    {
        tornPagesCollected++;
    }

    public void CompleteLevel()
    {
        isGameOver = true;
        Time.timeScale = 0f;

        // stop music
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.StopBGM();
        }

        if (PlayerData.Instance != null)
        {
            // transfer data to PlayerData.cs
            foreach (var kill in runKills)
            {
                var permanentData = PlayerData.Instance.GetEnemyKillData(kill.Key);
                permanentData.killAmount += kill.Value;
            }
            // add torn pages rewards
            PlayerData.Instance.AddTornPages(tornPagesCollected);
            Debug.Log("torn pages collected: " + tornPagesCollected);
        }
        else
        {
            Debug.Log("PlayerData Instance is null.");
        }

        if (SaveDataJSON.Instance != null)
        {
            // save to SaveDataJSON
            SaveDataJSON.Instance.SaveData();
        }

        if (UIManager.Instance != null)
        {
            // display end run screen
            UIManager.Instance.DisplayEndRunScreen();
        }
    }
}

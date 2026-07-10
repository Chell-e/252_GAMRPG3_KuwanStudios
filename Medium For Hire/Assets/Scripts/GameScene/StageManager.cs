using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// IN CHARGE OF THE CURRENT STAGE/LEVEL
public class StageManager : MonoBehaviour
{
    [SerializeField] public PlayerEvents Events;
    [SerializeField] private PoolSpawner poolSpawner;

    [SerializeField] private Timer timer;
    private float interval = 5f; // superstition checker interval = 2 minutes

    [SerializeField] private GameObject mapPrefab;

    public static StageManager Instance;
    public bool isGameOver = false;
    public bool isSuperstitionChecked = false;

    [Header("Superstition Checker")]
    public GameObject superstitionVerdict;
    public GameObject followedText;
    public GameObject violationText;
    public GameObject juddgementText;

    public static LevelData CurrentLevel { get; set; }
    public static int CurrentLevelRewards { get; set; }

    // temp storage for the current run
    private Dictionary<string, (int normal, int elite, int boss)> runKills = new Dictionary<string, (int, int, int)>();


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

        //if (CurrentLevel == null)
        //{
        //    Debug.Log("No level...");
        //    return;
        //}

        AssembleStage();
    }

    private void Update()
    {
        // secs muna 
        int seconds = Mathf.FloorToInt(timer.elapseTime % 60);

        if ((seconds / 5 > 0 && seconds % 5 == 0) && isSuperstitionChecked == false)
        {
            isSuperstitionChecked = true;
            DisplayVerdict();
        }
    }

    //private void CheckSuperstition(int violationCount)
    //{
    //    if (SuperstitionManager.Instance.activeSuperstition == null)
    //    {
    //        //juddgementText.SetActive(true);
    //    }
    //    else if (violationCount > 0)
    //    {
    //        // display superstition violation message
    //        //violationText.SetActive(true);
    //    }
    //    else
    //    {
    //        // display superstition success message
    //        //followedText.SetActive(true);
    //    }
    //}

    private void DisplayVerdict()
    {
        //superstitionVerdict.SetActive(true);
        Debug.Log("Checking superstition...");

        isSuperstitionChecked = false;
    }

    private void OnEnable()
    {
        isGameOver = false;

        Events.OnPlayerDeath += CompleteLevel;
        //SuperstitionManager.OnSuperstitionBroken += CheckSuperstition;

    }

    private void OnDisable()
    {
        Events.OnPlayerDeath -= CompleteLevel;
        //SuperstitionManager.OnSuperstitionBroken -= CheckSuperstition;

    }

    private void AssembleStage()
    {
        Instantiate(mapPrefab, Vector3.zero, Quaternion.identity);

        //// SET UP MAP
        //if (CurrentLevel.map.mapPrefab != null)
        //{
        //    Instantiate(CurrentLevel.map.mapPrefab, Vector3.zero, Quaternion.identity);
        //}

        //// SET UP POOL SPAWNER
        //if (poolSpawner != null)
        //{
        //    poolSpawner.SetUpLevelData(CurrentLevel);
        //}
        //else
        //{
        //    Debug.Log("Missing pool spawner reference.");
        //}

        //// SET UP SUPERSTITION
        //if (CurrentLevel.superstition != null)
        //{
        //    SuperstitionManager.Instance.ActivateSuperstition(CurrentLevel.superstition);
        //}
    }

    public void RegisterKill(string name, string type)
    {
        if (!runKills.ContainsKey(name))
            runKills[name] = (0, 0, 0);

        var currentRun = runKills[name];

        if (type == "Normal") currentRun.normal = currentRun.normal + 1;
        else if (type == "Elite") currentRun.elite = currentRun.elite + 1;
        else if (type == "Boss") currentRun.boss = currentRun.boss + 1;

        runKills[name] = currentRun;
    }

    public void CompleteLevel()
    {
        isGameOver = true;

        if (PlayerData.Instance != null)
        {
            // transfer data to PlayerData.cs
            foreach (var kill in runKills)
            {
                var permanentData = PlayerData.Instance.GetEnemyKillData(kill.Key);
                permanentData.normalKills += kill.Value.normal;
                permanentData.eliteKills += kill.Value.elite;
                permanentData.bossKills += kill.Value.boss;
            }
            // add pilon rewards
            PlayerData.Instance.AddPilon(CurrentLevelRewards);
            Debug.Log("current level rewards: " + CurrentLevelRewards);
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public static WeaponManager Instance;

    // for SINGLETON
    private void Awake()
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
    // for SINGLETON

    [Header("DEBUG")]
    [SerializeField] List<GameObject> subWeapons = new List<GameObject>();

    [Header("REFERENCES")]
    private PlayerController playerController;
    private PlayerStats playerStats;
    [Space]
    [SerializeField] public GameObject mainWeapon;
    [SerializeField] WeaponUnlock mainWeaponUIData;

    [SerializeField] public WeaponData mainWeaponData;


    // * DRIVER CODE
    // mainly Start() and Update()


    private void Start()
    {
        CheckPlayerReference();
        //EquipMainWeapon(StageManager.SelectedWeapon);

        //mainWeapon.GetComponent<BaseWeapon>().Initialize(playerController); // dont forget to Initialize() function 
        //UIManager.Instance.SetupMainWeaponSlot(mainWeaponUIData, mainWeapon.GetComponent<BaseWeapon>());

        if (mainWeaponData != null) EquipMainWeapon(mainWeaponData); // DONT FORGET TO REMOVE AFTER DEBUGGING
    }

    // * DRIVER CODE


    // *** CORE LOGIC
    // these are functions that coordinate smaller functions below
    public void EquipMainWeapon(WeaponData weaponData)
    {
        if (weaponData == null) return;

        CheckPlayerReference();
        mainWeapon = Instantiate(weaponData.weaponPrefab, transform);

        InitializeCurrentMainWeapon(weaponData);
    }
    // *** CORE LOGIC


    // ** SUB FUNCTIONS
    // more "individual" functions
    private void InitializeCurrentMainWeapon(WeaponData weaponData)
    {
        if (mainWeapon == null) return;

        CheckPlayerReference();
        mainWeapon.GetComponent<BaseWeapon>().Initialize(playerController);
        UIManager.Instance.SetupMainWeaponSlot(weaponData.mainWeaponUIData, mainWeapon.GetComponent<BaseWeapon>());
    }

    private void CheckPlayerReference()
    {
        if (playerController == null) playerController = PlayerController.Instance;
        if (playerController != null) playerStats = PlayerController.Instance.playerStats;
    }
    // ** SUB FUNCTIONS


    // TOOLS
    // external, getters/setters, non-method stuff (e.g., IEnumerator)
    public BaseWeapon AddMiniWeapon(GameObject weaponPrefab)
    {
        //// Check if already exists
        //foreach (var weapon in weapons)
        //{
        //    if (weapon.GetType() == weaponPrefab.GetType())
        //    {
        //        return;
        //    }
        //}

        // We need to also pass some sort of data to the UI Manager


        GameObject instance = Instantiate(weaponPrefab, transform);

        instance.GetComponent<BaseWeapon>().Initialize(playerController); // dont forget to Initialize() function 

        subWeapons.Add(instance);


        return instance.GetComponent<BaseWeapon>(); // return a reference to the added weapon
    }
    // TOOLS


    // EVENTS & LISTENERS
    // put events and listeners here

    // EVENTS & LISTENERS




}

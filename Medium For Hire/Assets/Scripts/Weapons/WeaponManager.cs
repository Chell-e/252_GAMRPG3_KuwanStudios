using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    [Header("DEBUG")]
    [SerializeField] List<GameObject> subWeapons = new List<GameObject>();

    [Header("REFERENCES")]
    private PlayerController playerController;
    private PlayerStats playerStats;
    [Space]
    [SerializeField] public GameObject mainWeapon;
    [SerializeField] WeaponUnlock mainWeaponUIData;

    // * DRIVER CODE
    // mainly Start() and Update()
    private void Awake()
    {
        
    }

    private void Start()
    {
        playerController = PlayerController.Instance;
        playerStats = PlayerController.Instance.playerStats;

        mainWeapon.GetComponent<BaseWeapon>().Initialize(playerController); // dont forget to Initialize() function 
        UIManager.Instance.SetupMainWeaponSlot(mainWeaponUIData, mainWeapon.GetComponent<BaseWeapon>());
    }

    // * DRIVER CODE


    // *** CORE LOGIC
    // these are functions that coordinate smaller functions below

    // *** CORE LOGIC


    // ** SUB FUNCTIONS
    // more "individual" functions

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

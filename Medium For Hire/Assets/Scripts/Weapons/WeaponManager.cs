using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private PlayerStats playerStats;

    [SerializeField] GameObject mainWeapon;
    [SerializeField] List<GameObject> subWeapons = new List<GameObject>();

    private void Awake()
    {
        //playerController = PlayerController.Instance;
        //playerStats = PlayerController.Instance.playerStats;
    }
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

}

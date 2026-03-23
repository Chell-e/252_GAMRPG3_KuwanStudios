using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private PlayerStats playerStats;

    [SerializeField] List<GameObject> weapons = new List<GameObject>();

    private void Awake()
    {
        //playerController = PlayerController.Instance;
        //playerStats = PlayerController.Instance.playerStats;
    }
    public void AddWeapon(GameObject weaponPrefab)
    {
        //// Check if already exists
        //foreach (var weapon in weapons)
        //{
        //    if (weapon.GetType() == weaponPrefab.GetType())
        //    {
        //        return;
        //    }
        //}


        GameObject instance = Instantiate(weaponPrefab, transform);

        instance.GetComponent<BaseWeapon>().Initialize(playerController); // dont forget to Initialize() function 

        weapons.Add(instance);
    }

}

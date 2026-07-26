using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WeaponSelectManager : MonoBehaviour
{
    public void SelectWeapon(WeaponData selectedWeaponData)
    {
        if (selectedWeaponData == null) return;

        // pass the selected weapon data to the stage manager
        StageManager.SelectedWeapon = selectedWeaponData;
        Debug.Log("Selected Weapon: " + selectedWeaponData.weaponName);
    }

    public void ConfirmSelectedWeapon()
    {
        if (StageManager.SelectedWeapon == null)
        {
            Debug.Log("hey! pick a weapon");
            return;
        }

        SceneManager.LoadScene("GameScene");
    }
}

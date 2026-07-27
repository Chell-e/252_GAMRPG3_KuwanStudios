using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WeaponSelectManager : MonoBehaviour
{
    [Header("REFERENCES")]
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI descriptionText;

    [SerializeField] TextMeshProUGUI normalText;
    [SerializeField] TextMeshProUGUI aimedText;

    public void SelectWeapon(WeaponData selectedWeaponData)
    {
        if (selectedWeaponData == null) return;

        // pass the selected weapon data to the stage manager
        StageManager.SelectedWeapon = selectedWeaponData;
        Debug.Log("Selected Weapon: " + selectedWeaponData.weaponTitle);

        // load the UI
        LoadInfo(selectedWeaponData);
    }

    private void LoadInfo(WeaponData selectedWeaponData)
    {
        nameText.text = selectedWeaponData.weaponTitle;
        descriptionText.text = selectedWeaponData.weaponDescription;
        normalText.text = selectedWeaponData.normalBehavior;
        aimedText.text = selectedWeaponData.aimedBehavior;

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

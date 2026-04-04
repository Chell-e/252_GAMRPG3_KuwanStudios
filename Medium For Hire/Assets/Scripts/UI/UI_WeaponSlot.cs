using UnityEngine;
using UnityEngine.UI;

public class UI_WeaponSlot : MonoBehaviour
{
        [Header("UI")]
    [SerializeField] private Image icon;
    [SerializeField] public Slider cooldownFill; // set this in editor

    [SerializeField] private BaseWeapon weaponData; // reference to weapon

    public TooltipTrigger tooltipTrigger;

    public void SetupSlot(WeaponUnlock _weaponUnlock, BaseWeapon _weaponData)
    {
        this.weaponData = _weaponData; // upgrade will pass the weapon's reference
        this.icon.sprite = _weaponUnlock.icon; // upgrade will pass its icon

        // find a way to implement events
        //weapon.OnCooldownChanged += UpdateUI;

        //tooltipTrigger.providerSource = _weaponData;
        tooltipTrigger.SetProvider(_weaponData);

        UpdateUI();
    }

    private void Update()
    {
        UpdateUI();
    }

    void UpdateUI()
    {
        // find a way to implement events

        if (weaponData == null) return;
        cooldownFill.value = weaponData.GetFillProgress();
        //Debug.Log(cooldownFill.value);
    }

    void OnDestroy()
    {
        
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("EXP UI")]
    [SerializeField] private Slider expSlider;
    [SerializeField] private TMP_Text expText;
    [SerializeField] private TMP_Text levelText;

    [Header("HP UI")]
    [SerializeField] private Slider hpSlider;
    [SerializeField] private TMP_Text hpText;

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


    public void UpdateExpSlider()
    {
        expSlider.maxValue = PlayerController.Instance.playerStats.expToLevel;
        expSlider.value = PlayerController.Instance.playerStats.currentEXP;
        expText.text = expSlider.value + " / " + expSlider.maxValue;

        levelText.text = "Level " + PlayerController.Instance.playerStats.currentLevel;

        //expSlider.maxValue = PlayerController.Instance.playerStats.expToLevelUp[PlayerController.Instance.playerStats.currentLevel - 1];
        //expSlider.value = PlayerController.Instance.playerStats.currentEXP;
        //expText.text = expSlider.value + " / " + expSlider.maxValue;    
    }


    public void UpdateHpSlider()
    {
        hpSlider.maxValue = PlayerController.Instance.playerStats.maxHealth;
        hpSlider.value = PlayerController.Instance.playerStats.currentHealth;

        hpText.text = hpSlider.value + " / " + hpSlider.maxValue;
    }
}

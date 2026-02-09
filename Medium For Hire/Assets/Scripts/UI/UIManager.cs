using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    [SerializeField] private Slider expSlider;
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
        //expSlider.maxValue = PlayerController.Instance.playerStats.maxLevel;
        //expSlider.value = PlayerController.Instance.playerStats.currentLevel;
    }
}

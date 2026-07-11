using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShrineUIManager : MonoBehaviour
{
    public static ShrineUIManager Instance;

        [Header("Shrine UI Panel Windows")]
    [SerializeField] private GameObject superstitionWindow;
    [SerializeField] private GameObject normalWindow;

        [Header("Normal Panel Texts")]
    [SerializeField] private TextMeshProUGUI normalTitleText;
    [SerializeField] private TextMeshProUGUI normalFlavorText;
    [SerializeField] private TextMeshProUGUI normalDescriptionText;

        [Header("Superstition Panel Texts")]
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI superstitionNameText;
    [SerializeField] private TextMeshProUGUI superstitionDescriptionText;
    [SerializeField] private TextMeshProUGUI superstitionRewardText;
    [SerializeField] private TextMeshProUGUI superstitionPenaltyText;

    private BaseShrine currentActiveContext = null;
    private GameObject currentActiveWindow = null;

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

    private void Start()
    {
        if (superstitionWindow != null)
            superstitionWindow.SetActive(false);

        if (normalWindow != null)
            normalWindow.SetActive(false);
    }

    public void OpenNormalPanel(BaseShrine shrine, string title, string flavorText, string description)
    {
        // pause time
        Time.timeScale = 0f;

        currentActiveContext = shrine;
        currentActiveWindow = normalWindow;

        // put text data in
        if (normalTitleText != null) normalTitleText.text = title;
        if (normalFlavorText != null) normalFlavorText.text = flavorText;
        if (normalDescriptionText != null) normalDescriptionText.text = description;

        // open it up
        if (currentActiveWindow != null) currentActiveWindow.SetActive(true);
    }

    public void OpenSuperstitionPanel(BaseShrine shrine, string title, SuperstitionData data)
    {
        // pause time
        Time.timeScale = 0f;

        currentActiveContext = shrine;
        currentActiveWindow = superstitionWindow;

        if (titleText != null) titleText.text = title;
        if (superstitionNameText != null) superstitionNameText.text = data.superstitionName;
        if (superstitionDescriptionText != null) superstitionDescriptionText.text = data.description;
        if (superstitionRewardText != null) superstitionRewardText.text = data.rewardText;
        if (superstitionPenaltyText != null) superstitionPenaltyText.text = data.penaltyText;

        if (currentActiveWindow != null) currentActiveWindow.SetActive(true);
    }

    public void OnAccept()
    {
        if (currentActiveContext == null) return;

        if (currentActiveContext is SpiritShrine spiritShrine) spiritShrine.ExecuteAccept();
        else if (currentActiveContext is AkasiShrine akasiShrine) akasiShrine.ExecuteAccept();
        else if (currentActiveContext is ApolakiShrine apolaki) apolaki.ExecuteAccept();

        CloseShrinePanel();
    }

    public void OnDecline()
    {
        if (currentActiveContext == null) return;

        if (currentActiveContext is SpiritShrine spiritShrine) spiritShrine.ExecuteDecline();
        else if (currentActiveContext is AkasiShrine akasiShrine) akasiShrine.ExecuteDecline();
        else if (currentActiveContext is ApolakiShrine apolaki) apolaki.ExecuteDecline();

        CloseShrinePanel();
    }

    public void CloseShrinePanel()
    {
        if (currentActiveWindow != null)
        {
            currentActiveWindow.SetActive(false);
        }

        currentActiveWindow = null;
        currentActiveContext = null;

        // unpause
        Time.timeScale = 1f;
    }
}
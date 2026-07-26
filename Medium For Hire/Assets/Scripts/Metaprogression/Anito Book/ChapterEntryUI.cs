using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class ChapterEntryUI : MonoBehaviour
{
    [SerializeField] private GameObject infoPage;
    [SerializeField] private GameObject storyPage;

        [Header("REFERENCES")]
    public Image enemySprite;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descriptionText;
    public Image illusHoveredFrame;
    public Image illusLock;
    public Image illusHoveredLock;
    public Image descFrame;
    public Image storyFrame;
    public Image storyLock;

    // * DRIVER CODE
    // mainly Start() and Update()
    public void LoadIllustration(ChapterEntry entry)
    {
        enemySprite.sprite = entry.chapterSprite;
        enemySprite.enabled = true;
    }

    public void LoadName(ChapterEntry entry)
    {
        nameText.text = entry.chapterName;
        nameText.enabled = true;
    }

    public void LoadDescription(ChapterEntry entry)
    {
        descriptionText.text = entry.chapterDescription;
        descriptionText.enabled = true;
    }

    public void UnloadIllustration(ChapterEntry entry)
    {
        enemySprite.sprite = null;
        enemySprite.enabled = false;
    }

    public void UnloadName(ChapterEntry entry)
    {
        nameText.text = null;
        nameText.enabled = false;
    }

    public void UnloadDescription(ChapterEntry entry)
    {
        descriptionText.text = null;
        descriptionText.enabled = false;
    }
    // * DRIVER CODE


    // *** CORE LOGIC
    // these are functions that coordinate smaller functions below

    // *** CORE LOGIC


    // ** SUB FUNCTIONS
    // more "individual" functions
    public void EnableHoveredPrimaryUnlockable()
    {
        illusHoveredFrame.enabled = true;
        illusHoveredLock.enabled = true;
        descFrame.enabled = true;
    }

    public void DisableHoveredPrimaryUnlockable()
    {
        illusHoveredFrame.enabled = false;
        illusHoveredLock.enabled = false;
        descFrame.enabled = false;
    }
    //public void 
    // ** SUB FUNCTIONS


    // TOOLS
    // external, getters/setters, non-method stuff (e.g., IEnumerator)

    // TOOLS


    // EVENTS & LISTENERS
    // put events and listeners here

    // EVENTS & LISTENERS


    private void Start()
    {
        //Setup(chapterEntry);
    }

    public void Setup(ChapterEntry entry)
    {
        //chapterEntry = entry;
        //creaturePage = _creaturePage;

        //bool isNameUnlocked = BestiaryManager.Instance.IsNameUnlocked(pageEntry);

        //nameText.text = isNameUnlocked ? pageEntry.name : "???";
        //descriptionText.text = isNameUnlocked ? pageEntry.chapterDescription : "Undiscovered.";
    }


    public void OnClick()
    {
        //creaturePage.SetCreaturePage(pageEntry);
    }
}

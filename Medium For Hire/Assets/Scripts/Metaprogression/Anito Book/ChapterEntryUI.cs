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
    public Image descHoverFrame;
    public Image storyHoverFrame;
    public Image storyHoverLock;
    public Image storyLock;
    public GameObject storyButton;
    public GameObject shortStoryPage;
    public TextMeshProUGUI storyPage1;
    public TextMeshProUGUI storyPage2;

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
        descHoverFrame.enabled = true;
    }

    public void DisableHoveredPrimaryUnlockable()
    {
        illusHoveredFrame.enabled = false;
        illusHoveredLock.enabled = false;
        descHoverFrame.enabled = false;
    }

    public void EnableHoveredSecondaryUnlockable()
    {
        storyHoverFrame.enabled = true;
        storyHoverLock.enabled = true;
    }

    public void DisableHoveredSecondaryUnlockable()
    {
        storyHoverFrame.enabled = false;
        storyHoverLock.enabled= false;
    }

    public void DisplayStoryPage(ChapterEntry _entry)
    {
        shortStoryPage.SetActive(true);

        if (storyPage1 == null || storyPage2 == null || string.IsNullOrEmpty(_entry.shortStory)) return;
        
        Canvas.ForceUpdateCanvases();
        storyPage1.text = _entry.shortStory;
        storyPage1.ForceMeshUpdate();

        if (storyPage1.isTextOverflowing)
        {
            int firsOverflowCharIndex = storyPage1.firstOverflowCharacterIndex;

            string storyPage1Content = _entry.shortStory.Substring(0, firsOverflowCharIndex);
            string storyPage2Content = _entry.shortStory.Substring(firsOverflowCharIndex);

            storyPage1.text = storyPage1Content.Trim();
            storyPage2.text = storyPage2Content.Trim();
        }
        else
        {
            storyPage2.text = "";
        }
    }

    public void CloseStoryPage()
    {
        if (storyPage != null) shortStoryPage.SetActive(false);
        if (infoPage != null) infoPage.SetActive(true);
    }
    //public void 
    // ** SUB FUNCTIONS


    // TOOLS
    // external, getters/setters, non-method stuff (e.g., IEnumerator)

    // TOOLS


    // EVENTS & LISTENERS
    // put events and listeners here

    // EVENTS & LISTENERS
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class PageEntryDisplay : MonoBehaviour
{
    public PageEntry pageEntry;
    public CreaturePageDisplay creaturePage;

    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descriptionText;

    private void Start()
    {
        Setup(pageEntry, creaturePage);
    }

    public void Setup(PageEntry _pageEntry, CreaturePageDisplay _creaturePage)
    {
        pageEntry = _pageEntry;
        creaturePage = _creaturePage;

        bool isNameUnlocked = BestiaryManager.Instance.IsNameUnlocked(pageEntry);

        nameText.text = isNameUnlocked ? pageEntry.name : "???";
        descriptionText.text = isNameUnlocked ? pageEntry.entryDescription : "Undiscovered.";
    }

    public void OnClick()
    {
        creaturePage.SetCreaturePage(pageEntry);
    }
}

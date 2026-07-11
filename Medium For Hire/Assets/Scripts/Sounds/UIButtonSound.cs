using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIButtonSounds : MonoBehaviour, IPointerEnterHandler
{
        [Header("Audio SFX Indexes")]
    [SerializeField] private int clickSoundIndex = 0;
    [SerializeField] private int hoverSoundIndex = 1;

        [Header("UI Interactions")]
    [SerializeField] private bool playOnClick = true;
    [SerializeField] private bool playOnHover = true;

    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
    }

    private void OnEnable()
    {
        if (button != null && playOnClick)
        {
            button.onClick.AddListener(PlayClickSound);
        }
    }

    private void OnDisable()
    {
        if (button != null)
        {
            button.onClick.RemoveListener(PlayClickSound);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!playOnHover) return;
        if (button != null && !button.interactable) return;

        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySFX(hoverSoundIndex);
        }
    }

    public void PlayClickSound()
    {
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySFX(clickSoundIndex);
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

        [Header("Audio Sources")]
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource sfxSource;

        [Header("Game Audio Clips")]
    [SerializeField] private List<AudioClip> bgmClips;
    [SerializeField] private List<AudioClip> sfxClips;

        [Header("Low BGM Settings")]
    [SerializeField] private float normalBGMVolume;
    [SerializeField] private float loweredBGMVolume = 0.2f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            transform.SetParent(null); // detaches from parent folder
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }

        normalBGMVolume = bgmSource.volume;
    }

    public void PlaySFX(int audioIndex)
    {
        //sfxSource.clip = sfxClips[audioIndex];
        sfxSource.PlayOneShot(sfxClips[audioIndex]);
    }

    public void PlayBGM(int audioIndex, bool isLoop)
    {
        bgmSource.clip = bgmClips[audioIndex];
        bgmSource.loop = isLoop;
        bgmSource.Play();
    }

    public void LowerBGM()
    {
        bgmSource.volume = loweredBGMVolume;
    }

    public void RevertBGM()
    {
        bgmSource.volume = normalBGMVolume;
    }

    public void StopBGM()
    {
        if (bgmSource != null && bgmSource.isPlaying)
        {
            bgmSource.Stop();
        }
    }
}

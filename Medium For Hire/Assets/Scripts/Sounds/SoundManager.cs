using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

        [Header("Audio Mixer")]
    [SerializeField] private AudioMixer mixer; 

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

            if (transform.parent != null)
            {
                transform.SetParent(null); // detaches from parent folder
            }
            DontDestroyOnLoad(gameObject);

            normalBGMVolume = bgmSource.volume;
            LoadSavedVolumes();
        }
        else
        {
            Destroy(gameObject);
        }

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

    public void LoadSavedVolumes()
    {
        float musicVolume = PlayerPrefs.GetFloat("musicVolume", 1f);
        float sfxVolume = PlayerPrefs.GetFloat("sfxVolume", 1f);

        SetMusicVolume(musicVolume);
        SetSFXVolume(sfxVolume);
    }

    public void SetMusicVolume(float volume)
    {
        float targetDb = volume > 0 ? Mathf.Log10(volume) * 20 : -80f;
        mixer.SetFloat("music", targetDb);
        PlayerPrefs.SetFloat("musicVolume", volume);
    }

    public void SetSFXVolume(float volume)
    {
        float targetDb = volume > 0 ? Mathf.Log10(volume) * 20 : -80f;
        mixer.SetFloat("sfx", targetDb);
        PlayerPrefs.SetFloat("sfxVolume", volume);
    }
}

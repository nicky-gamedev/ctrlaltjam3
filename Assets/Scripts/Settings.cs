using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    public GameObject panel;
    public AudioMixer audioMixer;
    public TMP_Dropdown resolutionDropdown;
    public Slider SFXSlider, MusicSlider;
    float currentSFX, currentMusic;
    Resolution[] resolutions;

    public void SetMusic(float volume)
    {
        audioMixer.SetFloat("Music", volume);
        currentSFX = volume;
    }
    public void SetSFX(float volume)
    {
        audioMixer.SetFloat("SFX", volume);
        currentMusic = volume;
    }
    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width,
                  resolution.height, Screen.fullScreen);
    }

    public void SaveSettings()
    {
        PlayerPrefs.SetInt("ResolutionPreference",
                   resolutionDropdown.value);
        PlayerPrefs.SetFloat("SFXPreference",
                   currentSFX);
        PlayerPrefs.SetFloat("MusicPreference",
           currentMusic);
    }

    public void LoadSettings(int currentResolutionIndex)
    {

        if (PlayerPrefs.HasKey("ResolutionPreference"))
            resolutionDropdown.value =
                         PlayerPrefs.GetInt("ResolutionPreference");
        else
            resolutionDropdown.value = currentResolutionIndex;

        if (PlayerPrefs.HasKey("SFXPreference"))
            SFXSlider.value =
                        PlayerPrefs.GetFloat("SFXPreference");
        else
            SFXSlider.value =
                        PlayerPrefs.GetFloat("SFXPreference");

        if (PlayerPrefs.HasKey("MusicPreference"))
            MusicSlider.value =
                        PlayerPrefs.GetFloat("MusicPreference");
        else
            MusicSlider.value =
                        PlayerPrefs.GetFloat("MusicPreference");


    }

    public void Close()
    {
        panel.SetActive(false);
    }

    void Awake()
    {
        resolutionDropdown.ClearOptions();
        List<string> options = new List<string>();
        resolutions = Screen.resolutions;
        int currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " +
                     resolutions[i].height;
            options.Add(option);
            if (resolutions[i].width == Screen.currentResolution.width
                  && resolutions[i].height == Screen.currentResolution.height)
                currentResolutionIndex = i;
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.RefreshShownValue();
        LoadSettings(currentResolutionIndex);
    }


}

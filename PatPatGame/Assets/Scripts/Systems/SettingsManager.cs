using System;
using UnityEngine;
using UnityEngine.UIElements;

public class SettingsManager : MonoBehaviour
{
    private SoundManager soundManager;
    private UISettingsFunc settingsFunc;
    public event Action<bool> OnHintToggleChanged;
    public event Action<int> OnMovementTypeChanged;

    void Start()
    {
        soundManager = GameManager.Instance.soundManager;
        settingsFunc = FindFirstObjectByType<UISettingsFunc>();
        if(settingsFunc != null)
        {
            settingsFunc.SetSettingsManager(this);
            SetComponents();
        }
    }

    void SetComponents()
    {
        if (PlayerPrefs.HasKey("HintToggle"))
        {
            int val = PlayerPrefs.GetInt("HintToggle");
            if(val == 1)
            {
                settingsFunc.SetHintToggle(true);
                OnHintToggleChanged?.Invoke(true);
            }
            else
            {
                settingsFunc.SetHintToggle(false);
                OnHintToggleChanged?.Invoke(false);
            }
        }
        if (PlayerPrefs.HasKey("MovementType"))
        {
            int val = PlayerPrefs.GetInt("MovementType");
            settingsFunc.SetMovementType(val);
            OnMovementTypeChanged?.Invoke(val);
        }

        if (PlayerPrefs.HasKey("MasterVolume"))
        {
            float audioValue;
            audioValue = PlayerPrefs.GetFloat("MasterVolume");
            settingsFunc.SetMasterSliderUI(audioValue);
            soundManager.SetMasterVolume(audioValue);
        }
        if (PlayerPrefs.HasKey("MusicVolume"))
        {
            float audioValue;
            audioValue = PlayerPrefs.GetFloat("MusicVolume");
            settingsFunc.SetMusicSliderUI(audioValue);
            soundManager.SetMusicVolume(audioValue);
        }
        if (PlayerPrefs.HasKey("SFXVolume"))
        {
            float audioValue;
            audioValue = PlayerPrefs.GetFloat("SFXVolume");
            settingsFunc.SetSFXSliderUI(audioValue);
            soundManager.SetSFXVolume(audioValue);
        }
        if (PlayerPrefs.HasKey("UIVolume"))
        {
            float audioValue;
            audioValue = PlayerPrefs.GetFloat("UIVolume");
            settingsFunc.SetUISliderUI(audioValue);
            soundManager.SetUIVolume(audioValue);
        }
    }

    public void MasterVolumeChanged(float value)
    {
        soundManager.SetMasterVolume(value);
        settingsFunc.SetMasterSliderUI(value);
        PlayerPrefs.SetFloat("MasterVolume", value);
    }

    public void MusicVolumeChanged(float value)
    {
        soundManager.SetMusicVolume(value);
        settingsFunc.SetMusicSliderUI(value);
        PlayerPrefs.SetFloat("MusicVolume", value);
    }

    public void SFXVolumeChanged(float value)
    {
        soundManager.SetSFXVolume(value);
        settingsFunc.SetSFXSliderUI(value);
        PlayerPrefs.SetFloat("SFXVolume", value);
    }

    public void UIVolumeChanged(float value)
    {
        soundManager.SetUIVolume(value);
        PlayerPrefs.SetFloat("UIVolume", value);
        settingsFunc.SetUISliderUI(value);
    }

    public void HintToggleChanged(bool value)
    {
        OnHintToggleChanged?.Invoke(value);
        int val = (value) ? 1: 0;
        PlayerPrefs.SetInt("HintToggle", val);
    }

    public void MovementTypeChanged(int value)
    {
        Debug.Log("MovementType: " + value);
        OnMovementTypeChanged?.Invoke(value);
        //PlayerPrefs.SetInt("MovementType", value);
    }
}

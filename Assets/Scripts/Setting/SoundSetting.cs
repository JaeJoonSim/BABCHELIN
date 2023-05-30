using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SoundSetting : MonoBehaviour
{
    public AudioMixer audioMixer;

    [Space]
    public Slider MasterVolumeSlider;
    public Slider BGMVolumeSlider;
    public Slider SFXVolumeSlider;

    [Space]
    public Button MasterVolumeButton;
    public Button BGMVolumeButton;
    public Button SFXVolumeButton;

    public void MasterVolumeButten()
    {
        MasterVolumeSlider.value = 0.0001f;
    }

    public void BGMVolumeButten()
    {
        BGMVolumeSlider.value = 0.0001f;
    }

    public void SFXVolumeButten()
    {
        SFXVolumeSlider.value = 0.0001f;
    }

    public void SetMasterVolume()
    {
        audioMixer.SetFloat("Master", Mathf.Log10(MasterVolumeSlider.value) * 20);
    }

    public void SetBGMVolume()
    {
        audioMixer.SetFloat("BGM", Mathf.Log10(BGMVolumeSlider.value) * 20);
    }

    public void SetSFXVolume()
    {
        audioMixer.SetFloat("SFX", Mathf.Log10(SFXVolumeSlider.value) * 20);
    }
}

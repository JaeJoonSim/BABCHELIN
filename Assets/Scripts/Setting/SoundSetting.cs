using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SoundSetting : MonoBehaviour
{
    public AudioMixer audioMixer;

    public Slider MasterVolumeSlider;
    public Slider BGMVolumeSlider;
    public Slider SFXVolumeSlider;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void VolumeToggle()
    {
        AudioListener.volume = AudioListener.volume == 0 ? 1 : 0;
    }
}

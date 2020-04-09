using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenuScript : MonoBehaviour
{
    public AudioSource VolumeMusic;

    private AudioManager volumeControl;

    public Slider musicSlider;
    public Slider soundFXSlider;

    // Start is called before the first frame update
    void Start()
    {
        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 1f);
        soundFXSlider.value = PlayerPrefs.GetFloat("SoundFXVolume", 1f);

        volumeControl = FindObjectOfType<AudioManager>();

        volumeControl.updateSoundVolume(soundFXSlider.value);
    }

    public void ChangeMusicSource()
    {

    }

    public void SetMusicVolume(float SliderValue)
    {
        VolumeMusic.volume = SliderValue;
        PlayerPrefs.SetFloat("MusicVolume", SliderValue);
    }

    void SetSoundFXVoume(float SliderValue)
    {
        volumeControl.updateSoundVolume(SliderValue);
        PlayerPrefs.SetFloat("SoundFXVolume", SliderValue);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenuScript : MonoBehaviour
{
    public AudioSource VolumeMusic;

    public AudioManager volumeControl;

    public Slider musicSlider;
    public Image mSliderArrowLeft, mSliderArrowRight;

    public Slider soundFXSlider;
    public Image sSliderArrowLeft, sSliderArrowRight;

    private float mSliderPrevious, sSliderPrevious;

    // Start is called before the first frame update
    void Start()
    {
        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 1f);
        soundFXSlider.value = PlayerPrefs.GetFloat("SoundFXVolume", 1f);

        mSliderPrevious = musicSlider.value;
        sSliderPrevious = soundFXSlider.value;

        volumeControl.updateSoundVolume(soundFXSlider.value);
    }

    public void ChangeMusicSource()
    {

    }

    public void SetMusicVolume(float SliderValue)
    {
        VolumeMusic.volume = SliderValue;
        PlayerPrefs.SetFloat("MusicVolume", SliderValue);

        if (musicSlider.value > mSliderPrevious)
        {
            mSliderArrowRight.color = new Color32(243, 238, 128, 255);
            mSliderPrevious = musicSlider.value;
            mSliderArrowLeft.color = new Color32(12, 193, 184, 255);
        }
        else if (musicSlider.value < mSliderPrevious)
        {
            mSliderArrowLeft.color = new Color32(243, 238, 128, 255);
            mSliderPrevious = musicSlider.value;
            mSliderArrowRight.color = new Color32(12, 193, 184,255);
        }
    }

    public void SetSoundFXVoume(float SliderValue)
    {
        volumeControl.updateSoundVolume(SliderValue);
        PlayerPrefs.SetFloat("SoundFXVolume", SliderValue);

        if (soundFXSlider.value > sSliderPrevious)
        {
            mSliderArrowRight.color = new Color32(243, 238, 128, 255);
            mSliderPrevious = soundFXSlider.value;
            mSliderArrowLeft.color = new Color32(12, 193, 184, 255);
        }
        else if (soundFXSlider.value < sSliderPrevious)
        {
            sSliderArrowLeft.color = new Color32(243, 238, 128, 255);
            sSliderPrevious = soundFXSlider.value;
            sSliderArrowRight.color = new Color32(12, 193, 184, 255);
        }
    }
}

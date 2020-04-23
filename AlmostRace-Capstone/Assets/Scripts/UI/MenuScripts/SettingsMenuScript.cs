using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SettingsMenuScript : MonoBehaviour
{
    public AudioSource VolumeMusic;

    public AudioManager volumeControl;

    public Slider musicSlider;
    public Image mSliderArrowLeft, mSliderArrowRight;

    public Slider soundFXSlider;
    public Image sSliderArrowLeft, sSliderArrowRight;

    public Slider aISlider;
    public Text onText, offText;

    private float mSliderPrevious, sSliderPrevious;
    private bool isStart = true;

    // Start is called before the first frame update
    void Start()
    {
        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 1f);
        soundFXSlider.value = PlayerPrefs.GetFloat("SoundFXVolume", 1f);
        aISlider.value = PlayerPrefs.GetFloat("AISpawning", 1f);

        mSliderPrevious = musicSlider.value;
        sSliderPrevious = soundFXSlider.value;

        volumeControl.updateSoundVolume(soundFXSlider.value);

        if (aISlider.value == 1)
        {
            offText.color = new Color32(243, 238, 128, 255);
            onText.color = new Color32(212, 212, 212, 255);
        }
        else
        {
            onText.color = new Color32(243, 238, 128, 255);
            offText.color = new Color32(212, 212, 212, 255);
        }

        ActivateColors();
    }

    private void ActivateColors()
    {
        isStart = false;
    }

    public void SetMusicVolume(float SliderValue)
    {
        VolumeMusic.volume = SliderValue;
        PlayerPrefs.SetFloat("MusicVolume", SliderValue);

        if (musicSlider.value > mSliderPrevious)
        {
            mSliderPrevious = musicSlider.value;
            if (isStart == false)
            {
                mSliderArrowRight.color = new Color32(243, 238, 128, 255);
                mSliderArrowLeft.color = new Color32(12, 193, 184, 255);
            }
        }
        else if (musicSlider.value < mSliderPrevious)
        {
            mSliderPrevious = musicSlider.value;
            if (isStart == false)
            {
            mSliderArrowLeft.color = new Color32(243, 238, 128, 255);
            mSliderArrowRight.color = new Color32(12, 193, 184,255);
            }
        }


    }

    public void SetSoundFXVoume(float SliderValue)
    {
        volumeControl.updateSoundVolume(SliderValue);
        PlayerPrefs.SetFloat("SoundFXVolume", SliderValue);

        if (soundFXSlider.value > sSliderPrevious)
        {
            sSliderPrevious = soundFXSlider.value;
            if (isStart == false)
            {
                sSliderArrowRight.color = new Color32(243, 238, 128, 255);
                sSliderArrowLeft.color = new Color32(12, 193, 184, 255);
            }
        }
        else if (soundFXSlider.value < sSliderPrevious)
        {
            sSliderPrevious = soundFXSlider.value;
            if (isStart == false)
            {
                sSliderArrowLeft.color = new Color32(243, 238, 128, 255);
                sSliderArrowRight.color = new Color32(12, 193, 184, 255);
            }
        }
    }

    public void ToggleAI(float SliderValue)
    {
        aISlider.value = SliderValue;
        PlayerPrefs.SetFloat("AISpawning", SliderValue);

        if (aISlider.value == 1)
        {
            offText.color = new Color32(243, 238, 128, 255);
            onText.color = new Color32(212, 212, 212, 255);
        }
        else
        {
            onText.color = new Color32(243, 238, 128, 255);
            offText.color = new Color32(212, 212, 212, 255);
        }
    }

    public void DeselectionEvent()
    {
        mSliderArrowLeft.color = new Color32(12, 193, 184, 255);
        mSliderArrowRight.color = new Color32(12, 193, 184, 255);
        sSliderArrowLeft.color = new Color32(12, 193, 184, 255);
        sSliderArrowRight.color = new Color32(12, 193, 184, 255);
    }
}

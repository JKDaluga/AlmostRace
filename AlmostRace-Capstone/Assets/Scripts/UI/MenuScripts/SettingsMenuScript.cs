using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SettingsMenuScript : MonoBehaviour
{
    AudioSource VolumeMusic;

    public AudioManager volumeControl;

    private DataManager AISwitch;

    public Slider musicSlider;
    public Image mSliderArrowLeft, mSliderArrowRight, mSliderDiamond;

    public Slider soundFXSlider;
    public Image sSliderArrowLeft, sSliderArrowRight, sSliderDiamond;

    public Slider aISlider;
    public Text onText, offText, backText;
    public Image aISliderDiamond;

    private float mSliderPrevious, sSliderPrevious;
    private bool isStart = true;

    public bool isMain;

    // Start is called before the first frame update
    void Start()
    {
        volumeControl = FindObjectOfType<AudioManager>();
        VolumeMusic = volumeControl.GetComponent<AudioSource>();

        AISwitch = FindObjectOfType<DataManager>();

        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 1f);
        soundFXSlider.value = PlayerPrefs.GetFloat("SoundFXVolume", 1f);
        
        if (isMain == true)
        aISlider.value = PlayerPrefs.GetFloat("AISpawning", 1f);

        mSliderPrevious = musicSlider.value;
        sSliderPrevious = soundFXSlider.value;

        volumeControl.updateSoundVolume(soundFXSlider.value);

        if (isMain == true)
        {
            if (aISlider.value == 1)
            {
                onText.color = new Color32(243, 238, 128, 255);
                offText.color = new Color32(212, 212, 212, 255);
                AISwitch.ChangeAISpawn(true);
            }
            else
            {
                offText.color = new Color32(243, 238, 128, 255);
                onText.color = new Color32(212, 212, 212, 255);
                AISwitch.ChangeAISpawn(false);
            }
        }

        ActivateColors();

        if(isMain==true)
        gameObject.SetActive(false);
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
                StartCoroutine(SelectionEvent(mSliderArrowRight));
                mSliderArrowLeft.color = new Color32(12, 193, 184, 255);
            }
        }
        else if (musicSlider.value < mSliderPrevious)
        {
            mSliderPrevious = musicSlider.value;
            if (isStart == false)
            {
                StartCoroutine(SelectionEvent(mSliderArrowLeft));
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
                StartCoroutine(SelectionEvent(sSliderArrowRight));
                sSliderArrowLeft.color = new Color32(12, 193, 184, 255);
            }
        }
        else if (soundFXSlider.value < sSliderPrevious)
        {
            sSliderPrevious = soundFXSlider.value;
            if (isStart == false)
            {
                StartCoroutine(SelectionEvent(sSliderArrowLeft));
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
            onText.color = new Color32(243, 238, 128, 255);
            offText.color = new Color32(212, 212, 212, 255);
            AISwitch.ChangeAISpawn(true);
        }
        else
        {
            offText.color = new Color32(243, 238, 128, 255);
            onText.color = new Color32(212, 212, 212, 255);
            AISwitch.ChangeAISpawn(false);
        }
    }

    public void ChangeSelection(GameObject target)
    {
        EventSystem.current.SetSelectedGameObject(target);
    }

    public void DeselectionEvent()
    {
        mSliderArrowLeft.color = new Color32(12, 193, 184, 255);
        mSliderArrowRight.color = new Color32(12, 193, 184, 255);
        sSliderArrowLeft.color = new Color32(12, 193, 184, 255);
        sSliderArrowRight.color = new Color32(12, 193, 184, 255);
        backText.color = new Color32(42, 63, 68, 255);
    }

    public void SelectionEvent()
    {
        backText.color = new Color32(243, 238, 128, 255);
    }

    public void StartUpdate()
    {
        SetSoundFXVoume(soundFXSlider.value);
        SetMusicVolume(musicSlider.value);
    }

    IEnumerator SelectionEvent(Image litUpImage)
    {
        litUpImage.color = new Color32(243, 238, 128, 255);

        yield return new WaitForSeconds(.1f);

        litUpImage.color = new Color32(12, 193, 184, 255);
    }
}

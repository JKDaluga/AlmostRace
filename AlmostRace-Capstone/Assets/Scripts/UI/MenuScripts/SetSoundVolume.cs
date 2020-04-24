using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetSoundVolume : MonoBehaviour
{
    public SettingsMenuScript SoundVolume;

    private bool isStart = true;
    

    //Just a dummy function that is meant to allow a slider to transfer its value to the settings menu
    public void SetVolume(float SliderValue)
    {
        SoundVolume.SetSoundFXVoume(SliderValue);

        if (isStart == false)
        {
            AudioManager.instance.PlayWithoutSpatial("Menu Selection");
        }

        if (isStart == true)
        {
            ActivateSound();
        }
    }

    void ActivateSound()
    {
        isStart = false;
    }
}

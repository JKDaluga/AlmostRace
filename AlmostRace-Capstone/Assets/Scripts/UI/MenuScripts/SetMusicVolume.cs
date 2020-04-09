using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetMusicVolume : MonoBehaviour
{
    public SettingsMenuScript musicVolume;

    //Just a dummy function that is meant to allow a slider to transfer its value to the settings menu
    public void SetVolume(float SliderValue)
    {
        musicVolume.SetMusicVolume(SliderValue);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetSoundVolume : MonoBehaviour
{
    public SettingsMenuScript SoundVolume;

    //Just a dummy function that is meant to allow a slider to transfer its value to the settings menu
    public void SetVolume(float SliderValue)
    {
        SoundVolume.SetSoundFXVoume(SliderValue);
        
            AudioManager.instance.Play("Menu Selection", transform);
        
    }
}

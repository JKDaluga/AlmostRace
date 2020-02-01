using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EngineAudio : MonoBehaviour
{
    AudioManager audioManager;
    public RaycastCar car;
    bool playAudio = true;
    AudioSource engineSound;
    public float topPitch = 1;
    public float bottomPitch = 0;

    // Start is called before the first frame update
    void Start()
    {
        if(AudioManager.instance != null)
        {
            audioManager = AudioManager.instance;
        }
        else
        {
            audioManager = GameObject.FindObjectOfType<AudioManager>();
        }
        if(audioManager == null)
        {
            Debug.LogError("AudioManager Can Not be Found");
        }
        else
        {
            this.gameObject.transform.position = audioManager.gameObject.transform.position;
        }
        engineSound = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if(playAudio)
        {
            engineSound.pitch = ExtensionMethods.Remap(car.currentSpeed / car.maxSpeed, 0, 1, bottomPitch, topPitch);
        }
    }

    public void toggleEngine(bool toggle)
    {
        playAudio = toggle;
        if(playAudio)
        {
            engineSound.Play();
        }
        else
        {
            engineSound.Stop();
        }
    }
}

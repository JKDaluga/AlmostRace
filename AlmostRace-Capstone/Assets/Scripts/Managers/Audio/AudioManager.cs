using UnityEngine.Audio;
using System;
using System.Collections.Generic;

using UnityEngine;

/*
Eddie B. / Jason D. / Jake V.

Directly using Brackey's tutorial found at https://www.youtube.com/watch?v=6OT43pvUyfY
     
    Something I'm not entirely fond of is that all of the sounds are 
    stored in one list. This should be expanded later somehow, so that designers 
    don't have to look at a 400+ element list of sounds. Perhaps catagories such as "abilities", "environment", and 
    "music" can be set up, but even those might be too broad.

Jason/Jake heavily edited and changed this to suit our new audio needs
*/

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    private AudioSource source;

    public Sound[] sounds;

    [Header("Pseudo-Spatial Sound Variables")]
    public float innerSoundDistance = 0f;
    public float maxSoundDistance = 0f;
    private RaceManager _raceManager;

    void Awake()
    {
        if (instance != null)
        {
            source = GetComponent<AudioSource>();
            if(instance.source.clip != source.clip)
            {
                instance.source.clip = source.clip;
                if(instance.source.clip != null)
                {
                    instance.source.Play();
                }
            }
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            source = GetComponent<AudioSource>();
            if(source == null)
            {
                Debug.LogError("No Audio Source");
            }
            DontDestroyOnLoad(gameObject);
        }
        if(_raceManager == null)
        {
            _raceManager = FindObjectOfType<RaceManager>();
        }
    }

    public void Play(string sound, Transform soundTransform)
    {
        
        Sound s = FindSound(sound);

        if(sound == null)
        {
            return;
        }

        float spatialVolume = s.volume;
        
        if (_raceManager != null)
        {
            float distance = Mathf.Infinity;
            foreach (RaycastCar car in _raceManager.cars)
            {
                if(car != null)
                {
                    float tempDistance = Vector3.Distance(car.transform.position, soundTransform.position);
                    if (distance > tempDistance)
                    {
                        distance = tempDistance;
                        if (distance <= innerSoundDistance)
                        {
                            spatialVolume = s.volume;
                        }
                        else if (distance <= maxSoundDistance)
                        {
                            spatialVolume = s.volume * ((distance - innerSoundDistance) / (maxSoundDistance - innerSoundDistance));
                        }
                        else
                        {
                            spatialVolume = 0;
                        }
                    }
                }
            }
        }

        source.PlayOneShot(s.clip, spatialVolume);
    }

    public void PlayWithoutSpatial(string sound)
    {

        Sound s = FindSound(sound);

        if (sound == null)
        {
            return;
        }

        source.PlayOneShot(s.clip, s.volume);
    }

    public Sound FindSound(string sound)
    {
        Sound s = Array.Find(sounds, item => item.name == sound);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return null;
        }
        return s;
    }
}

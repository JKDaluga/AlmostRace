using UnityEngine.Audio;
using System;
using UnityEngine;

/*
Eddie B.

Directly using Brackey's tutorial found at https://www.youtube.com/watch?v=6OT43pvUyfY
     
    Something I'm not entirely fond of is that all of the sounds are 
    stored in one list. This should be expanded later somehow, so that designers 
    don't have to look at a 400+ element list of sounds. Perhaps catagories such as "abilities", "environment", and 
    "music" can be set up, but even those might be too broad.
*/

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    public AudioSource source;

    public Sound[] sounds;

    void Awake()
    {
        if (instance != null)
        {
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
    }

    public void Play(string sound)
    {
        Sound s = FindSound(sound);

        if(sound == null)
        {
            return;
        }

        source.PlayOneShot(s.clip, s.volume);
    }

    public void Stop(string sound)
    {
        Sound s = FindSound(sound);
        
        if (sound == null)
        {
            return;
        }

        s.source.Stop();
    }

    public void SetSoundPitchAndVolume(string sound, float volume, float pitch)
    {
        Sound s = FindSound(sound);

        if (sound == null)
        {
            return;
        }

        s.source.volume = s.volume * (1f + UnityEngine.Random.Range(-s.volumeVariance / 2f, s.volumeVariance / 2f));
        s.source.pitch = s.pitch * (1f + UnityEngine.Random.Range(-s.pitchVariance / 2f, s.pitchVariance / 2f));
    }

    public void SetSoundPitchAndVolume(Sound sound, float volume, float pitch)
    {
        sound.source.volume = sound.volume * (1f + UnityEngine.Random.Range(-sound.volumeVariance / 2f, sound.volumeVariance / 2f));
        sound.source.pitch = sound.pitch * (1f + UnityEngine.Random.Range(-sound.pitchVariance / 2f, sound.pitchVariance / 2f));
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

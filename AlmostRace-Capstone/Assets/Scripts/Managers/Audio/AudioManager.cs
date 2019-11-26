using UnityEngine.Audio;
using System;
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

using UnityEngine.Audio;
using UnityEngine;

/*
Eddie B. / Jason D. / Jake V

Directly using Brackey's tutorial found at https://www.youtube.com/watch?v=6OT43pvUyfY

Jason/Jake heavily edited and changed this to suit our new audio needs
*/
[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;
    [Range(0f, 1f)]
    public float volume = .75f;
}

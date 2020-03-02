using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DriftSound : MonoBehaviour
{
    private AudioManager audioManager;

    // Start is called before the first frame update
    void Start()
    {
        if (AudioManager.instance != null)
        {
            audioManager = AudioManager.instance;
        }
        else
        {
            audioManager = GameObject.FindObjectOfType<AudioManager>();
        }
        if (audioManager == null)
        {
            Debug.LogWarning("AudioManager Can Not be Found");
        }
        else
        {
            this.gameObject.transform.position = audioManager.gameObject.transform.position;
        }
    }
}

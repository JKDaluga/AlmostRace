using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayCredits : MonoBehaviour
{

    public AudioManager creditsAudio;
    

    void Start()
    {
        creditsAudio = FindObjectOfType<AudioManager>();
        
    }

    public void ChangeAudio(string C)
    {
       creditsAudio.PlayWithoutSpacial()
        
    }


}

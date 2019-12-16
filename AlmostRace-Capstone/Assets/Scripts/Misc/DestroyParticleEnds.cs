using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    Creator and developer of script: Jake Velicer
    Purpose: Destorys the object once its particle effect is done
*/
public class DestroyParticleEnds : MonoBehaviour
{
    private ParticleSystem thisParticleSystem;

    // Start is called before the first frame update
    void Start()
    {
        thisParticleSystem = gameObject.GetComponent<ParticleSystem>();
        float totalDuration = thisParticleSystem.main.startLifetime.constant;
        Destroy(gameObject, totalDuration);
    }

}

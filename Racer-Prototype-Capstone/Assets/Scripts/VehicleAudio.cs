using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleAudio : MonoBehaviour
{
    public CarPhysicsBehavior vehicleScript;
    public AudioSource audioSource;
    public float speedDivide;
    public float minPitch;
    public float maxPitch;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        audioSource.pitch = Mathf.Clamp(vehicleScript.currentDriveForce / speedDivide, minPitch, maxPitch);
    }
}

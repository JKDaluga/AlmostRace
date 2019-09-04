using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleFlameParticleBehavior : MonoBehaviour
{
    public CarPhysicsBehavior theVehicleScript;
    private ParticleSystem particles;
    private float particleSpeed;

    // Start is called before the first frame update
    void Start()
    {
        particles = GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        particleSpeed = theVehicleScript.currentDriveForce / 225;
        particles.startSpeed = Mathf.Clamp(particleSpeed, 0.8f, 4);
    }
}

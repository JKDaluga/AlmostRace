using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindParticleBehavior : MonoBehaviour
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
        particleSpeed = theVehicleScript.currentDriveForce / 2;
        particles.startSpeed = particleSpeed;
    }
}

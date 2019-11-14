using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 Eddie B

    Simple script for the flame pillars. They are meant to be a one hit kill and hyper lethal.
    Activated only after a turret is destroyed.
     
     */

public class TurretFirePillarBehavior : MonoBehaviour
{
    private Collider pillarCollider;
    public ParticleSystem pillarParticles;

    private void Start()
    {
        pillarCollider = gameObject.GetComponent<Collider>();
        Deactivate();
    }

    public void Activate()
    {
        pillarCollider.enabled = true;
        pillarParticles.Play();
    }

    public void Deactivate()
    {
        pillarCollider.enabled = false;
        pillarParticles.Stop();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.GetComponent<CarHeatManager>() != null)
        { //If a car runs into the flame pillar, blow up that car.
            other.gameObject.GetComponent<CarHeatManager>().AddHeat(other.gameObject.GetComponent<CarHeatManager>().heatExplodeLimit);
        }
    }

}

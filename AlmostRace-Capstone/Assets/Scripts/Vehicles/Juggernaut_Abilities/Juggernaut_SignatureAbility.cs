using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 Eddie B

    This script handles the boost-like ability for the Juggernaut.
     
     
     */

public class Juggernaut_SignatureAbility : Ability
{

    public SphereCarController sphereCarScript;
    public float boostSpeed;
    public float boostDuration;
    public float newSteering;
    private float _originalSteering;

    private void Start()
    {
        _originalSteering = sphereCarScript.steering;
    }

    public override void ActivateAbility()
    {
        sphereCarScript.steering = newSteering;
        sphereCarScript.SetBoostSpeed(boostSpeed);
        sphereCarScript.SetIsBoosting(true);
        Invoke("DeactivateAbility", boostDuration);
    }

    public override void DeactivateAbility()
    {
        sphereCarScript.steering = _originalSteering;
        sphereCarScript.SetBoostSpeed(0);
        sphereCarScript.SetIsBoosting(false);
    }
}

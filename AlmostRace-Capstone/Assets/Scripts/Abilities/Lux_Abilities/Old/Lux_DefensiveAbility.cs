using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    Eddie B

    The defensive ability for the Lux.
    Spawns a number of shield panels around the Lux that protect it for a set number of damage.  
     */

public class Lux_DefensiveAbility : CooldownAbility
{
    public float shieldHealth;
    public List<GameObject> _shields;
    private CarHealthBehavior _carHealthScript;
 
    void Start()
    {
        _carHealthScript = gameObject.GetComponent<CarHealthBehavior>();
        foreach(GameObject shield in _shields)
        {
            shield.GetComponent<MeshRenderer>().enabled = false;
        }
    }

    public override void ActivateAbility()
    { //DeactivateAbility called VehicleAbilityBehavior after the duration is over.
        AudioManager.instance.Play("Shield Activated");
        _carHealthScript.SetExtraHealth(shieldHealth);
        foreach (GameObject shield in _shields)
        {
            shield.GetComponent<MeshRenderer>().enabled = true;
        }
    }

    public override void DeactivateAbility()
    {
        _carHealthScript.SetExtraHealth(0);
        foreach (GameObject shield in _shields)
        {
            shield.GetComponent<MeshRenderer>().enabled = false;
        }
    }

}

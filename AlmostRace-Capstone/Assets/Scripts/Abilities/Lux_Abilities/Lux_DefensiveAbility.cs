﻿using System.Collections;
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
 
    void Start()
    {
        foreach(GameObject shield in _shields)
        {
            shield.GetComponent<Lux_ShieldPanelBehavior>().GiveInfo(shieldHealth, gameObject);
        }
    }

    public override void ActivateAbility()
    { //DeactivateAbility called VehicleAbilityBehavior after the duration is over.
        foreach (GameObject shield in _shields)
        {
            shield.GetComponent<Lux_ShieldPanelBehavior>().TriggerInteractable();
        }
    }

    public override void DeactivateAbility()
    {
        foreach (GameObject shield in _shields)
        {
            shield.GetComponent<Lux_ShieldPanelBehavior>().DestroyInteractable();
        }
    }

}

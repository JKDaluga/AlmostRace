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
    private CarHeatManager _carHealthScript;
 
    void Start()
    {
        _carHealthScript = gameObject.GetComponent<CarHeatManager>();
        foreach(GameObject shield in _shields)
        {
         
        }
    }

    public override void ActivateAbility()
    { //DeactivateAbility called VehicleAbilityBehavior after the duration is over.
        AudioManager.instance.Play("Shield Activated");
        _carHealthScript.SetExtraHealth(shieldHealth);
        _carHealthScript.SetExtraHealthMax(shieldHealth);
        foreach (GameObject shield in _shields)
        {
            shield.GetComponent<Lux_ShieldPanelBehavior>().GiveInfo(_carHealthScript);
            shield.GetComponent<Lux_ShieldPanelBehavior>().TriggerInteractable();     
            
        }
    }

    public override void DeactivateAbility()
    {
        _carHealthScript.SetExtraHealth(0);
        _carHealthScript.SetExtraHealthMax(0);
        foreach (GameObject shield in _shields)
        {
            shield.GetComponent<Lux_ShieldPanelBehavior>().DestroyInteractable();
          
        }
    }

}

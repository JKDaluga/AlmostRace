using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 Author: Eddie Borissov
 Purpose: Handles the Volt's medium range shock attack by turning the attack on and off.
 Turns on while holding the fire button, turns off when the button is released.
 Lightning cone damage handled in a separate script.
     
     */




public class Volt_BasicAbility : BasicAbility
{

    public GameObject lightningCone;
    private Volt_LightningCone _voltLightningConeInfo;

    [Header("Ability Values")]

    [Tooltip("How much Hype is given per lightning bolt every X seconds")]
    public float lightningHypeToGain; 

    [Tooltip("How much damage is done per lightning bolt every X seconds")]
    public float lightningDamage;

    [Tooltip("How often enemy cars within range are hit with lightning. Lower number means more frequent!")]
    public float lightningFrequency;


    private void Start()
    {
        carHeatInfo = gameObject.GetComponent<CarHeatManager>();
        _voltLightningConeInfo = lightningCone.GetComponent<Volt_LightningCone>();
        _voltLightningConeInfo.SetImmunePlayer(gameObject);
        _voltLightningConeInfo.SetLightningDamageAndFrequency(lightningDamage, selfHeatDamage, lightningFrequency, lightningHypeToGain);
        lightningCone.SetActive(false); // Off at the start of the game.
    }

    public override void ActivateAbility()
    {
        if (lightningCone.activeSelf == false)
        {
            InvokeRepeating("AddHeat", 0, lightningFrequency);
            lightningCone.SetActive(true);
        }
      
    }

    public override void DeactivateAbility()
    {
        if(lightningCone.activeSelf == true)
        {
            CancelInvoke("AddHeat");
            _voltLightningConeInfo.EndAbility();//ensures list of enemies kept by the cone gets cleared.
            lightningCone.SetActive(false);
        }
    }

    protected override void AddHeat()
    {
        carHeatInfo.heatCurrent += selfHeatDamage;
    }
}

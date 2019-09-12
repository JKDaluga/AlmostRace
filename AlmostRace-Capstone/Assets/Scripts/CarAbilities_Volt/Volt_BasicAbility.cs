using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 Author: Eddie Borissov
 Purpose: Handles the Volt's medium range shock attack by turning the attack on and off.
 Turns on while holding the fire button, turns off when the button is released.
 Lightning cone damage handled in a separate script.
     
     */




public class Volt_BasicAbility : Ability
{

    public GameObject lightningCone;
    private Volt_LightningCone _voltLightningConeInfo;

    [Header("Ability Values")]
    [Tooltip("How much damage is done per lightning bolt every X seconds")] public float lightningDamage;
    [Tooltip("How much damage this ability does to its own car every X seconds")] public float selfDamage;
    [Tooltip("How often enemy cars within range are hit with lightning. Lower number means more frequent!")] public float lightningFrequency;


    private void Start()
    {
        _voltLightningConeInfo = lightningCone.GetComponent<Volt_LightningCone>();
        _voltLightningConeInfo.SetImmunePlayer(gameObject);
        _voltLightningConeInfo.SetLightningDamageAndFrequency(lightningDamage, selfDamage, lightningFrequency);
        lightningCone.SetActive(false); // Off at the start of the game.
    }

    public override void ActivateAbility()
    {
        if (lightningCone.activeSelf == false)
        {
            lightningCone.SetActive(true);
        }
    }

    public override void DeactivateAbility()
    {
        if(lightningCone.activeSelf == true)
        {
            lightningCone.SetActive(false);
        }
    }


}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

/*
 Eddie B. This code is basically a copy past of Volt_LaserBolt (which will probably get deleted soon).
 I made this script to allow us to edit behavior while keeping a temporary backup.
 This script will no doubt become the final version eventually.
     */

public class Lux_ProjectileBehavior : Projectile
{
    private void Start()
    {
        base.Start();
        GiveSpeed();
    }

    private void OnTriggerEnter(Collider other)
    {   
        if (_isAlive)
        {
            if (other.gameObject != _immunePlayer && other.gameObject.GetComponent<CarHeatManager>() != null)
            {//Checks if the object isn't the immunePlayer and if they are a car.
                other.gameObject.GetComponent<CarHeatManager>().DamageCar(_projectileDamage);

                other.gameObject.GetComponent<CinemachineImpulseSource>().m_ImpulseDefinition.m_AmplitudeGain = .75f;
                other.gameObject.GetComponent<CinemachineImpulseSource>().m_ImpulseDefinition.m_FrequencyGain = .75f;
                other.gameObject.GetComponent<CinemachineImpulseSource>().GenerateImpulse();
                _immunePlayerScript.AddHype(_projectileHype, "Damage:");
                AudioManager.instance.Play("Bullet Impact Lux");
                //Debug.Log("Projectile destoryed by:" + other.gameObject.name);

                StartCoroutine(ExplosionEffect());
            }
            else if (other.gameObject != _immunePlayer && other.gameObject.GetComponent<Interactable>() != null)
            {//Checks if the object isn't the immunePlayer and if they are an interactable object.
                other.gameObject.GetComponent<Interactable>().interactingPlayer = _immunePlayer;
                other.gameObject.GetComponent<Interactable>().DamageInteractable(_projectileDamage);
                
               // Debug.Log("Projectile destoryed by:" + other.gameObject.name);

                StartCoroutine(ExplosionEffect());
            }
            else if (other.gameObject != _immunePlayer)
            {
                //Debug.Log("Projectile destoryed by:" + other.gameObject.name);
                StartCoroutine(ExplosionEffect());
            }
        }
    }
}

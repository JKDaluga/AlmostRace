using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

/*
 Eddie B
    Simple code for the Turret projectiles. They are currently behaving like lasers, can be changed to anything.
     
     */

public class TurretProjectileBehavior : Projectile
{
    private GameObject _aggroObject;
    public void Start()
    {
        GiveSpeed();
    }

    public void SetProjectileInfo(float projectileDamage, float projectileSpeed, GameObject immunePlayer, GameObject aggroObject)
    {
        _projectileDamage = projectileDamage;
        _projectileSpeed = projectileSpeed;
        _immunePlayer = immunePlayer;
        _aggroObject = aggroObject;
    }

    private void OnTriggerEnter(Collider other)
    {

        Debug.Log("laserbolt collided with: " + other.gameObject.name);
        if (other.gameObject.GetComponent<Interactable>() != null && other.gameObject != _aggroObject)
        {//Checks if the object isn't the immunePlayer and if they are an interactable object.

            if (!other.gameObject.CompareTag("Projectile"))
            {
                //  Debug.Log("Damage done to: " + other.gameObject.name + " " + _projectileDamage);
                if (other.gameObject.GetComponent<TurretBehavior>() == null)
                {
                   // Debug.Log("object hit:" + other.gameObject.name);
                    other.gameObject.GetComponent<Interactable>().DamageInteractable(_projectileDamage);
                    Destroy(gameObject);
                    // StartCoroutine(ExplosionEffect());
                }
            }
            StartCoroutine(ExplosionEffect());
        }
        else if (other.gameObject.GetComponent<CarHealthBehavior>() != null)
        {//if other is a car
            other.gameObject.GetComponent<CarHealthBehavior>().DamageCar(_projectileDamage, 100);
            other.gameObject.GetComponent<CinemachineImpulseSource>().m_ImpulseDefinition.m_AmplitudeGain = .4f;
            other.gameObject.GetComponent<CinemachineImpulseSource>().m_ImpulseDefinition.m_FrequencyGain = .4f;

            other.gameObject.GetComponent<CinemachineImpulseSource>().GenerateImpulse();
            
            //Debug.Log("Damage done to player: " + _projectileDamage);
            StartCoroutine(ExplosionEffect());
        }
        else
        {
            Destroy(gameObject, 10);
        }


    }

}

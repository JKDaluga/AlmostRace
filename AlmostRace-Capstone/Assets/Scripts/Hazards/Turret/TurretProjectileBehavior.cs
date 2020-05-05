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
    private ObjectPooler _objectPooler;
    public void Start()
    {
        GiveSpeed();
    }

    public void SetProjectileInfo(float projectileDamage, float projectileSpeed, GameObject immunePlayer, GameObject aggroObject, ObjectPooler objPooler)
    {
        _projectileDamage = projectileDamage;
        _projectileSpeed = projectileSpeed;
        _immunePlayer = immunePlayer;
        _aggroObject = aggroObject;
        _objectPooler = objPooler;
    }

    private void OnTriggerEnter(Collider other)
    {

        //Debug.Log("laserbolt collided with: " + other.gameObject.name);
        if (other.gameObject.GetComponent<Interactable>() != null && other.gameObject != _aggroObject)
        {//Checks if the object isn't the immunePlayer and if they are an interactable object.

            if (!other.gameObject.CompareTag("Projectile"))
            {
                //  Debug.Log("Damage done to: " + other.gameObject.name + " " + _projectileDamage);
                if (other.gameObject.GetComponent<TurretBehavior>() == null)
                {
                    // Debug.Log("object hit:" + other.gameObject.name);
                    other.gameObject.GetComponent<Interactable>().DamageInteractable(_projectileDamage);

                    GameObject spawnedImpactVFX = _objectPooler.SpawnFromPool("LaserImpactSparks", transform.position, transform.rotation);
                    _objectPooler.DeactivateAfterTime("LaserImpactSparks", spawnedImpactVFX, 1);

                    Destroy(gameObject);
                    // StartCoroutine(ExplosionEffect());
                }
            }

            GameObject spawnedImpactVFX2 = _objectPooler.SpawnFromPool("LaserImpactSparks", transform.position, transform.rotation);
            _objectPooler.DeactivateAfterTime("LaserImpactSparks", spawnedImpactVFX2, 1);
            StartCoroutine(ExplosionEffect());
        }
        else if (other.gameObject.GetComponent<CarHealthBehavior>() != null)
        {//if other is a car
            other.gameObject.GetComponent<CarHealthBehavior>().DamageCar(_projectileDamage, 100);

            if(other.GetComponent<VehicleInput>())
            {
                other.gameObject.GetComponent<CinemachineImpulseSource>().m_ImpulseDefinition.m_AmplitudeGain = .4f;
                other.gameObject.GetComponent<CinemachineImpulseSource>().m_ImpulseDefinition.m_FrequencyGain = .4f;
                other.gameObject.GetComponent<CinemachineImpulseSource>().GenerateImpulse();
            }


            //Debug.Log("Damage done to player: " + _projectileDamage);


            GameObject spawnedImpactVFX = _objectPooler.SpawnFromPool("LaserImpactSparks", transform.position, transform.rotation);
            _objectPooler.DeactivateAfterTime("LaserImpactSparks", spawnedImpactVFX, 1);

            StartCoroutine(ExplosionEffect());
        }
        else if (other.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {

            GameObject spawnedImpactVFX = _objectPooler.SpawnFromPool("LaserImpactSparks", transform.position, transform.rotation);
            _objectPooler.DeactivateAfterTime("LaserImpactSparks", spawnedImpactVFX, 1);

            StartCoroutine(ExplosionEffect());
        }
        else
        {
            Destroy(gameObject, 10);
        }


    }

}

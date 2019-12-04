using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 
    Simple code for the Turret projectiles. They are currently behaving like lasers, can be changed to anything.
     
     */

public class TurretProjectileBehavior : Projectile
{

    public new void Start()
    {
        base.Start();
        GiveSpeed();
    }

    public void SetProjectileInfo(float projectileDamage, float projectileSpeed, GameObject immunePlayer)
    {
        _projectileDamage = projectileDamage;
        _projectileSpeed = projectileSpeed;
        _immunePlayer = immunePlayer;
    }

    private void OnTriggerEnter(Collider other)
    {
       
            if (other.gameObject.GetComponent<CarHeatManager>() != null)
            {//if other is a car
                other.gameObject.GetComponent<CarHeatManager>().AddHeat(_projectileDamage);
                Debug.Log("Damage done to player: " + _projectileDamage);
                StartCoroutine(ExplosionEffect());
            }
            else if (other.gameObject.GetComponent<Interactable>() != null)
            {//Checks if the object isn't the immunePlayer and if they are an interactable object.

            if(other.gameObject.CompareTag("Projectile"))
            {
                if (other.gameObject.GetComponent<TurretBehavior>() == null)
                {
                    other.gameObject.GetComponent<Interactable>().DamageInteractable(_projectileDamage);
                    StartCoroutine(ExplosionEffect());
                }
            }
            }
            else
            {
                StartCoroutine(ExplosionEffect());
            }
        
       
    }

}

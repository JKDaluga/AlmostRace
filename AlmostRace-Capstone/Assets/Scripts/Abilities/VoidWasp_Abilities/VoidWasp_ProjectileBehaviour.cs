using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoidWasp_ProjectileBehaviour : Projectile
{
    private void OnTriggerEnter(Collider other)
    {
        if (_isAlive)
        {
            if (other.gameObject != _immunePlayer && other.gameObject.GetComponent<CarHeatManager>() != null)
            {//Checks if the object isn't the immunePlayer and if they are a car.
                other.gameObject.GetComponent<CarHeatManager>().AddHeat(_projectileDamage);
                _immunePlayerScript.AddHype(_projectileHype, "Damage:");
                StartCoroutine(ExplosionEffect());
            }
            else if (other.gameObject != _immunePlayer && other.gameObject.GetComponent<Interactable>() != null)
            {//Checks if the object isn't the immunePlayer and if they are an interactable object.
                if (other.gameObject.GetComponent<VoidWasp_ShieldBehaviour>() != null)
                {//checks if about to hit a shield
                    if (other.gameObject.GetComponent<VoidWasp_ShieldBehaviour>().GetShieldPlayer() == _immunePlayer)
                    {
                        return;
                    }
                }

                other.gameObject.GetComponent<Interactable>().interactingPlayer = _immunePlayer;
                other.gameObject.GetComponent<Interactable>().DamageInteractable(_projectileDamage);
                StartCoroutine(ExplosionEffect());
            }
            else if (other.gameObject != _immunePlayer)
            {
                Debug.Log(gameObject.name + " has collided with: " + other.gameObject.name);
                StartCoroutine(ExplosionEffect());
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Mike Romeo 08/10/19
 * BoosterPadBehaviour gets the SphereCarController topspeed and accelaration variables 
 * and multiplies them with a percentage.
 */

public class BoosterPadBehaviour : Interactable
{
    private float _originalTopSpeed;
    private float _originalAccelarion;

    [Tooltip("Set the percentage of the boost can increase speed")]
    [Range(-100, 100)]
    public float speedMultiplier;

    [Tooltip("Set the percentage of the boost can increase acceleration")]
    [Range(0, 100)]
    public float accelMultiplier;

    [Tooltip("Set the boosting particle effect")]
    public GameObject boostEffect;

    private void OnTriggerEnter(Collider other)
    {
    
        if(other.gameObject.GetComponent<SphereCarController>() != null)
        {
            // Save original values so we can reset it later 
            _originalTopSpeed = other.gameObject.GetComponent<SphereCarController>().topSpeed;
            _originalAccelarion = other.gameObject.GetComponent<SphereCarController>().acceleration;

            // Increase max values of speed and acceleration 
            other.gameObject.GetComponent<SphereCarController>().topSpeed += ((speedMultiplier * _originalTopSpeed) / 100);
            other.gameObject.GetComponent<SphereCarController>().acceleration += ((accelMultiplier * _originalAccelarion) / 100);

            //Debug.Log("og speed: " + _originalTopSpeed + " new speed: " + other.gameObject.GetComponent<SphereCarController>().topSpeed);
            //Debug.Log("og accel: " + _originalAccelarion + "new accel: " + other.gameObject.GetComponent<SphereCarController>().acceleration);

           // GameObject newBoostEffect = Instantiate(boostEffect, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
            //newBoostEffect.transform.parent = other.transform;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<SphereCarController>() != null)
        {
            // Reset the original values when leaving the boost pad
            other.gameObject.GetComponent<SphereCarController>().topSpeed = _originalTopSpeed;
            other.gameObject.GetComponent<SphereCarController>().acceleration = _originalAccelarion;
            
            //Debug.Log("og speed: " + _originalTopSpeed + " new speed: " + other.gameObject.GetComponent<SphereCarController>().topSpeed);
            //Debug.Log("og accel: " + _originalAccelarion + "new accel: " + other.gameObject.GetComponent<SphereCarController>().acceleration);

            /*foreach (Transform boosterChild in other.transform)
            {
                if (boosterChild.name == "newBoostEffect")
                {
                    Debug.Log("Child found. Mame: " + boosterChild.name);
                }
            }*/
        }
    }

    public override void DamageInteractable(float damageNumber)
    {

    }

    public override void DestroyInteractable()
    {

    }

    public override void ResetInteractable()
    {

    }

    public override void TriggerInteractable()
    {

    }
}

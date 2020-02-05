using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Mike Romeo 08/10/19
 * BoosterPadBehaviour gets the SphereCarController topspeed and accelaration variables 
 * and multiplies them with a percentage.
 * 
 * Activates a boosting particle effect when player is on the boost pad.
 */

public class BoosterPadBehaviour : Interactable
{
    private float _originalTopSpeed;
    private float _originalAccelarion;

    [Tooltip("Set the percentage of the boost can increase speed")]
    [Range(-100, 100)]
    public float speedMultiplier;
    private List<CarHealthBehavior> _carsBoosted = new List<CarHealthBehavior>();

    private AudioSource powerUpSound;

   /* [Tooltip("Set the boosting particle effect")]
    public GameObject boostEffect;*/

    private void OnTriggerEnter(Collider other)
    {
    
        if(other.gameObject.GetComponent<SphereCarController>() != null)
        {
            _carsBoosted.Add(other.gameObject.GetComponent<CarHealthBehavior>());
            if(_carsBoosted.Count == 1)
            {
                StartCoroutine(TrackCars());
            }
            other.gameObject.GetComponent<SphereCarController>().SetBoosterPadInfo(speedMultiplier);
            other.gameObject.GetComponent<SphereCarController>().SetIsOnBoosterPad(true);

            powerUpSound = other.gameObject.GetComponent<AudioSource>();

            // Turn on the particle effect
            other.gameObject.GetComponent<SphereCarController>().boostingParticles.Play();
        }
    }

    private IEnumerator TrackCars()
    {
        while(_carsBoosted.Count > 1)
        {
            foreach(CarHealthBehavior car in _carsBoosted)
            {
                if(car.isDead)
                {
                    _carsBoosted.Remove(car);
                }
            }
            yield return null;
        }

        StopAllCoroutines();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<SphereCarController>() != null)
        {
            // Reset the original values when leaving the boost pad
            other.gameObject.GetComponent<SphereCarController>().SetIsOnBoosterPad(false);
            _carsBoosted.Remove(other.gameObject.GetComponent<CarHealthBehavior>());

            // Turn off the boost effect
            other.gameObject.GetComponent<SphereCarController>().boostingParticles.Stop();
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

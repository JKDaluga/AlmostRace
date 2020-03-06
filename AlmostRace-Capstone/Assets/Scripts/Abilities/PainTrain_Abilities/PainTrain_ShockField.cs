using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PainTrain_ShockField : MonoBehaviour
{
    public PainTrain_LightningBall lightningBall;

    private Interactable tempInteractable;

    private CarHealthBehavior _immunePlayer;

    private void Start()
    {
        _immunePlayer = gameObject.GetComponent<CarHealthBehavior>();
    }

    public void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Vehicle") && other.gameObject != lightningBall.getImmunePlayer())
        {
            Debug.Log(other.name + " was detected! " + other.transform.parent.name);

            lightningBall.AddCarToDamage(other.GetComponent<CarHealthBehavior>());

            if(lightningBall.GetTargetCar() == null) //IF the car isn't magnetizing to a car, magnetize
            {
                lightningBall.SetTargetCar(other.transform);
            }
        }

        if(other.CompareTag("Interactable"))
        {
            tempInteractable = other.GetComponent<Interactable>(); //this line should save us a "getcomponent" call

            if (tempInteractable.canBeDamaged)
            { // we want this to simply trigger interactables
                tempInteractable.DamageInteractable(tempInteractable.interactableHealth); //should insta-kill interactables
            }
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Vehicle") && other.gameObject != lightningBall.getImmunePlayer())
        {
            Debug.Log(other.name + " was detected! " + other.transform.parent.name);

            lightningBall.RemoveCarFromDamage(other.GetComponent<CarHealthBehavior>());
        }
    }


}

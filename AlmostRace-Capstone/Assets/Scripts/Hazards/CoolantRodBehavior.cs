using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoolantRodBehavior : Interactable
{
   

    public override void TriggerInteractable()
    {
       //cause AOE explosion
       //particle effects
       //activate fire lanes
    }

    public override void ResetInteractable()
    {
        //make rod damagable
        //turn on the rod's collider
        //make the rod go up
  
    }

    public override void DestroyInteractable()
    {   
        //make rod not damagable
        //make coolant rod go into the ground
        //make coolant rod's collider turn off
        //turn off mesh render

    }

    public override void DamageInteractable(float damageNumber)
    {
        if (canBeDamaged)
        {
            interactableHealth -= damageNumber;
            if(interactableHealth <= 0)
            {
                DestroyInteractable();
            }
        }
    }
}

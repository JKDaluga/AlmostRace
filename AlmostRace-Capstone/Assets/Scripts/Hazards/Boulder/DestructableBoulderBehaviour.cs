using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Mike Romeo 06/11/2019
 * Functionality for destuctable boulder that the player can shoot at
 * or collide with. 
 * 
 * Edited by Eddie B
 * Made to work with turrets blowing up boulders as well.
 */

public class DestructableBoulderBehaviour : Interactable
{
    [Tooltip("Amount of damage it does to the vehicle.")]
    public int ramDamage = 20;

    [Tooltip("Amount of hype gained by destroying the boulder.")]
    public float boulderDestroyedHype = 50f;

    [Tooltip("Amount that ramming the boulder slows the player.")]
    public float slowDownFactor = 4;

    [Tooltip("Attach boulder particle effect.")]
    public ParticleSystem boulderParticles;

    private MeshRenderer rend;
    private Collider coll;

    [Tooltip("A reference to destruction sound.")]
    public AudioClip deathSound;

    // Start is called before the first frame update
    void Start()
    {
        rend = this.GetComponent<MeshRenderer>();
        coll = this.GetComponent<Collider>();
        rend.enabled = true;
        coll.enabled = true;
    }

    private void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.GetComponent<CarHeatManager>() != null)
        {
           // if(canBeDamaged)
          //  {
            interactingPlayer = collision.gameObject; // sets the person crashing with the boulder as the interacting player
            TriggerInteractable();
            if(collision.gameObject.GetComponent<AimAssistant>().aimCircle.GetComponent<AimCollider>().colliding.Contains(gameObject))
            {
                collision.gameObject.GetComponent<AimAssistant>().aimCircle.GetComponent<AimCollider>().colliding.Remove(gameObject);
            }
            collision.gameObject.GetComponent<CarHeatManager>().DamageCar(ramDamage);
            if (collision.gameObject.GetComponent<SphereCarController>() != null)
            {
                collision.gameObject.GetComponent<SphereCarController>().currentSpeed -=
                    (collision.gameObject.GetComponent<SphereCarController>().currentSpeed / slowDownFactor);
            }
           // }

        }
    }

    public override void TriggerInteractable()
    {
        if(interactingPlayer != null)
        {
            if (interactingPlayer.GetComponent<VehicleHypeBehavior>() != null)
            {//makes sure that non-player agents can destroy the boulders without throwing null references.
                interactingPlayer.GetComponent<VehicleHypeBehavior>().AddHype(boulderDestroyedHype, "Vandal");
            }
        }

        if(interactingPlayer.GetComponent<AimAssistant>().target = gameObject)
        {
            interactingPlayer.GetComponent<AimAssistant>().aimCircle.GetComponent<AimCollider>().colliding.Remove(gameObject);
        }

     
        boulderParticles.Play();
        rend.enabled = false;
        coll.enabled = false;

        AudioManager.instance.Play("RockExplosion");

        Invoke("DestroyInteractable", boulderParticles.main.startLifetime.constant);
    }

    public override void DestroyInteractable()
    {
        Destroy(transform.parent.gameObject);
    }

    public override void ResetInteractable()
    {
        //Do not need
    }

    public override void DamageInteractable(float damageNumber)
    {
        interactableHealth -= damageNumber;
        if (interactableHealth <= 0)
        {
            TriggerInteractable();
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoulderBehaviour : Interactable
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

        if (collision.gameObject.GetComponent<CarHealthBehavior>() != null)
        {

            interactingPlayer = collision.gameObject; // sets the person crashing with the boulder as the interacting player
            TriggerInteractable();
            if (collision.gameObject.GetComponent<VehicleInput>())
            {
                if (collision.gameObject.GetComponent<AimAssistant>().aimCircle.GetComponent<AimCollider>().colliding.Contains(gameObject))
                {
                    collision.gameObject.GetComponent<AimAssistant>().aimCircle.GetComponent<AimCollider>().colliding.Remove(gameObject);
                }
            }
            if (collision.gameObject.GetComponent<AIBehaviour>())
            {
                collision.gameObject.GetComponentInChildren<AIObstacleAvoidance>().turnR = false;
                collision.gameObject.GetComponentInChildren<AIObstacleAvoidance>().turnL = false;
            }

            collision.gameObject.GetComponent<CarHealthBehavior>().DamageCar(ramDamage);
            if (collision.gameObject.GetComponent<SphereCarController>() != null)
            {
                collision.gameObject.GetComponent<SphereCarController>().currentSpeed -=
                    (collision.gameObject.GetComponent<SphereCarController>().currentSpeed / slowDownFactor);
            }

        }
    }

    public override void TriggerInteractable()
    {
        if (interactingPlayer != null)
        {
            if (interactingPlayer.GetComponent<VehicleInput>())
            {
                if (interactingPlayer.GetComponent<AimAssistant>().target == gameObject)
                {
                    interactingPlayer.GetComponent<AimAssistant>().aimCircle.GetComponent<AimCollider>().colliding.Remove(gameObject);
                    interactingPlayer.GetComponent<AimAssistant>().aimCircle.GetComponent<AimCollider>().interactables.Remove(GetComponent<Collider>());
                }
            }

            if (interactingPlayer.GetComponent<AIBehaviour>())
            {
                interactingPlayer.GetComponentInChildren<AIObstacleAvoidance>().turnR = false;
                interactingPlayer.GetComponentInChildren<AIObstacleAvoidance>().turnL = false;
            }
        }
        AimCollider[] allPlayers = FindObjectsOfType<AimCollider>();

        foreach (AimCollider i in allPlayers)
        {
            i.interactables.Remove(GetComponent<Collider>());
        }

        boulderParticles.Play();
        rend.enabled = false;
        coll.enabled = false;

        AudioManager.instance.Play("RockExplosion", transform);
        Invoke("DestroyInteractable", boulderParticles.main.startLifetime.constant);
    }

    public override void DestroyInteractable()
    {

        if (interactingPlayer != null)
        {
            if (interactingPlayer.GetComponent<AIBehaviour>())
            {
                interactingPlayer.GetComponentInChildren<AIObstacleAvoidance>().turnR = false;
                interactingPlayer.GetComponentInChildren<AIObstacleAvoidance>().turnL = false;
            }
        }

        Destroy(gameObject.transform.parent.gameObject);
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

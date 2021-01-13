using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    Creator and developer of script: Eddie Borrisov
    Purpose: Handles code for coolant rod interactable
*/
public class CoolantRodBehavior : Interactable
{
    [Header("Coolant Rod Variables............................................................")]
    [Space(30)]
   // public ParticleSystem coolantExplosion;

    [Tooltip("How long it takes for a new Coolant Rod to appear")]
    public float coolantRodCooldown = 5f;

    [Tooltip("How much hype to add to player that destroys the Coolant Rod")]
    public float coolantExplosionHype = 100f;

    private float _originalHealth;



    [Header("Animation Variables............................................................")]
    [Space(30)]
    public GameObject objToSlide;
    public GameObject columns;
    [Tooltip("How far up the Rod can go.")]
    public Transform topLimit;
    [Tooltip("How far down the Rod can go.")]
    public Transform bottomLimit;
    [Tooltip("How far the Rod slides per Rate.")]
    public float slideAmount = 1f;
    [Tooltip("How often the Rod slides.")]
    public float slideRate = .1f;
    private int slideDirection = 1; // 1 for up, -1 for down
    

    [Header("Coolant Line Variables............................................................")]
    [Space(30)]
    public List<CoolantLineBehavior> coolantLines;
    public float coolantLineDuration = 3f;
    public float coolantLineDamage = 5f;
    public float coolantLineRate = .25f;

    [Tooltip("Hype gained by damaging others with coolant fire.")]
    public float fireDamageHype;
    [Tooltip("Hype gained by killing others with coolant fire.")]
    public float fireKillHype;

    public void Start()
    {
        _originalHealth = interactableHealth;
        canBeDamaged = true;
        foreach (CoolantLineBehavior coolantLine in coolantLines)
        {
            coolantLine.InitializeCoolantLine(coolantLineDuration, coolantLineDamage, coolantLineRate, fireDamageHype, fireKillHype);
        }
        InvokeRepeating("RotateSphere", 0, .01f);
    }

    public void RotateSphere()
    {
        objToSlide.transform.Rotate(0, 1, 0);
    }

    public void RotateColumns()
    {
        columns.transform.Rotate(0, 1, 0);
    }

public void SlideRod()
    {
        if (objToSlide.gameObject.transform.localPosition.y > topLimit.localPosition.y) //Checks if the Rod went too high. Corrects if it did.
        {
            //Corrects position
            objToSlide.gameObject.transform.localPosition = new Vector3(objToSlide.gameObject.transform.localPosition.x, topLimit.localPosition.y, (objToSlide.gameObject.transform.localPosition.z));
            //Stops slide
            CancelInvoke("SlideRod");
        }
        else if (objToSlide.gameObject.transform.localPosition.y < bottomLimit.localPosition.y)
        {
            //Corrects position
            objToSlide.gameObject.transform.localPosition = new Vector3(objToSlide.gameObject.transform.localPosition.x, bottomLimit.localPosition.y, (objToSlide.gameObject.transform.localPosition.z));
            //Stops slide
            foreach (CoolantLineBehavior coolantLine in coolantLines)
            {
                coolantLine.ActivateCoolantLine(interactingPlayer);
                //Coolant lines disable themselves based on the duration passed in their initialization method.
            }
            CancelInvoke("SlideRod");
        }
        else
        {
            objToSlide.gameObject.transform.Translate(0, slideAmount * slideDirection, 0);
        }
    }

    public override void TriggerInteractable()
    {
        /*
        if(interactingPlayer != null)
        {
            if (interactingPlayer.GetComponent<AimAssistant>().target == gameObject)
            {
                interactingPlayer.GetComponent<AimAssistant>().aimCircle.GetComponent<AimCollider>().colliding.Remove(gameObject);
            }
        }
        */

        AimCollider[] allPlayers = FindObjectsOfType<AimCollider>();

        foreach (AimCollider i in allPlayers)
        {
            i.interactables.Remove(GetComponent<Collider>());
        }

    }

    public override void ResetInteractable()
    {
        canBeDamaged = true;//make rod damagable
        slideDirection = 1;
        AimCollider[] allPlayers = FindObjectsOfType<AimCollider>();

        foreach (AimCollider i in allPlayers)
        {
            i.interactables.Add(GetComponent<Collider>());
        }
        InvokeRepeating("SlideRod", 0, slideRate); //make the rod go up
        AudioManager.instance.Play("Cooling Rod Up", transform);
        interactableHealth = _originalHealth;
    }

    public override void DestroyInteractable()
    {
        canBeDamaged = false;//make rod not damagable
        slideDirection = -1;
        InvokeRepeating("SlideRod", 0, slideRate);//make coolant rod go into the ground
       // InvokeRepeating("RotateColumns", 0, .01f);
        Invoke("ResetInteractable", coolantRodCooldown);
    }

    public override void DamageInteractable(float damageNumber)
    {
        if (canBeDamaged)
        {
            interactableHealth -= damageNumber;
            if (interactableHealth <= 0)
            {
                TriggerInteractable();
                DestroyInteractable();
            if (interactingPlayer != null)
                {
                    interactingPlayer = null; // Resets interactingPlayer, just in case. Might have to reset for fire too, not sure yet.   
                }
            }
        }
    }
}

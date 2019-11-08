using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoolantRodBehavior : Interactable
{
    [Header("Coolant Rod Variables")]
    public ParticleSystem coolantExplosion;
 
    [Tooltip("How long it takes for a new Coolant Rod to appear")]
    public float coolantRodCooldown = 5f;
    private float _originalHealth;

    [Space(30)]

    [Header("Animation Variables")]
    [Tooltip("How far up the Rod can go.")]
    public Transform topLimit;
    [Tooltip("How far down the Rod can go.")]
    public Transform bottomLimit;
    [Tooltip("How far the Rod slides per Rate.")]
    public float slideAmount = 1f;
    [Tooltip("How often the Rod slides.")]
    public float slideRate = .1f;
    private int slideDirection = 1; // 1 for up, -1 for down
    [Space(30)]

    [Header("Coolant Line Variables")]
    public List<CoolantLineBehavior> coolantLines;
    public float coolantLineDuration = 3f;
    public float coolantLineDamage = 5f;
    public float coolantLineRate = .25f;

    public void Start()
    {
        _originalHealth = interactableHealth;
        canBeDamaged = true;
        foreach (CoolantLineBehavior coolantLine in coolantLines)
        {
            Debug.Log("Initialize Coolant Line was activated on: " + coolantLine.gameObject.name);
            coolantLine.InitializeCoolantLine(coolantLineDuration, coolantLineDamage, coolantLineRate);
        }
    }

    public void SlideRod()
    {
        if(gameObject.transform.localPosition.y > topLimit.localPosition.y) //Checks if the Rod went too high. Corrects if it did.
        {
            //Corrects position
            gameObject.transform.localPosition = new Vector3(gameObject.transform.localPosition.x, topLimit.localPosition.y, (gameObject.transform.localPosition.z));
            //Stops slide
            CancelInvoke("SlideRod"); 
        }
        else if (gameObject.transform.localPosition.y < bottomLimit.localPosition.y)
        {
            //Corrects position
            gameObject.transform.localPosition = new Vector3(gameObject.transform.localPosition.x, bottomLimit.localPosition.y, (gameObject.transform.localPosition.z));
            //Stops slide
            CancelInvoke("SlideRod");
        }
        else
        {
            gameObject.transform.Translate(0, slideAmount * slideDirection, 0);
        }
           
    }

    public override void TriggerInteractable()
    {
        coolantExplosion.Play();
        foreach (CoolantLineBehavior coolantLine in coolantLines)
        {
            Debug.Log("Activate Coolant Line was activated on: " + coolantLine.gameObject.name);
            coolantLine.ActivateCoolantLine();
            //Coolant lines disable themselves based on the duration passed in their initialization method.
        }
    }

    public override void ResetInteractable()
    {
        canBeDamaged = true;//make rod damagable
        slideDirection = 1;
        InvokeRepeating("SlideRod", 0, slideRate); //make the rod go up
        interactableHealth = _originalHealth;


    }

    public override void DestroyInteractable()
    {

        canBeDamaged = false;//make rod not damagable
        slideDirection = -1;
        InvokeRepeating("SlideRod", 0, slideRate);//make coolant rod go into the ground
        Invoke("ResetInteractable", coolantRodCooldown);


    }

    public override void DamageInteractable(float damageNumber)
    {
        if (canBeDamaged)
        {
            interactableHealth -= damageNumber;
            if(interactableHealth <= 0)
            {
                TriggerInteractable();
                DestroyInteractable();
            }
        }
    }
}

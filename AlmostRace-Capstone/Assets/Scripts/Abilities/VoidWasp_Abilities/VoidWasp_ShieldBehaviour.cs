using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    Mike Romeo
    - Document Created: 21/11/2019

    Title:  Void Surge
    The defensive ability for the void wasp.
    Activates a circle collider that collects all the damage 
    done to the car and keeps track of it. 
    When over it will release the damage in a AOE effect.  
    
*/

public class VoidWasp_ShieldBehaviour : Interactable
{

    public GameObject psRef;

    private float _maxHealth;
    private GameObject _surgeObject;

    private Collider _collider;
    private MeshRenderer _meshRender;

    private float _collectedDamage;

    private bool _released;

    // Start is called before the first frame update
    void Start()
    {
        canBeDamaged = true;
        psRef.SetActive(false);
        _collider = gameObject.GetComponent<Collider>();
        _meshRender = gameObject.GetComponent<MeshRenderer>();
        DestroyInteractable();
    }

    public void GiveInfo(float maxHealth, GameObject shieldRef)
    {
        interactableHealth = maxHealth;
        _surgeObject = shieldRef;
    }

    public override void DamageInteractable(float damageNumber)
    {
        interactableHealth -= damageNumber;
        _collectedDamage += damageNumber;
        if (interactableHealth < 0)
        {
            DestroyInteractable();
        }

        print("shield heath: " + interactableHealth);
    }

    public override void DestroyInteractable()
    {
        if (_released)
        {
            //_meshRender.enabled = false;

            _collider.enabled = false;
            psRef.SetActive(false);
        }
        if (!_released)
        {

        }



        // Call function that repels the damage again
        // Seperate objects activates as trigger? 
        // Does damage to any car that is around it OR shoot out random projectiles? 
        // Could be more wasps maybe, and these projectiles do damage based in total 
        // collected damage divided over total amount projectiles
    }

    public override void ResetInteractable()
    {
        interactableHealth = _maxHealth;
        _collectedDamage = 0;
    }

    public override void TriggerInteractable()
    {
        ResetInteractable(); // makes sure each panel has full health.
        //_meshRender.enabled = true;
        _collider.enabled = true;
        psRef.SetActive(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<Projectile>() != null)
        {

            Destroy(other.gameObject);

            print("Getting hit by turret");
        }

    }
}

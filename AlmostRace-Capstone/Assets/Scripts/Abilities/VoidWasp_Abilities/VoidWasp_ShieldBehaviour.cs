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

    private SphereCollider _collider;
    private MeshRenderer _meshRender;

    private float _collectedDamage;

    private float _explosionRadius;

    private bool _released;

    private GameObject _immunePlayer;

    Collider[] objectsHit;

    // Start is called before the first frame update
    void Start()
    {
        canBeDamaged = true;
        psRef.SetActive(false);
        _collider = gameObject.GetComponent<SphereCollider>();
        _collider.enabled = false;
        _meshRender = gameObject.GetComponent<MeshRenderer>();
        //_meshRender.enabled = false;
        DestroyInteractable();
    }

    public void GiveInfo(float maxHealth, GameObject shieldRef, float explosionRadius, GameObject immunePlayer)
    {
        interactableHealth = maxHealth;
        _surgeObject = shieldRef;
        _explosionRadius = explosionRadius;
       // Debug.Log("1 explosion radius: " + _explosionRadius);
        _immunePlayer = immunePlayer;
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

        _meshRender.enabled = false;
        _collider.enabled = false;
        psRef.GetComponent<Animator>().SetBool("surgeEnd", true);

        // Call function that repels the damage again
        // Seperate objects activates as trigger? 
        // Does damage to any car that is around it OR shoot out random projectiles? 
        // Could be more wasps maybe, and these projectiles do damage based in total 
        // collected damage divided over total amount projectiles
    }



    private void SurgeRelease()
    {
        psRef.GetComponent<Animator>().SetTrigger("startVoidSurge");
        print("release" + _released);

    }

    public override void ResetInteractable()
    {
        interactableHealth = _maxHealth;
        _collectedDamage = 0;
    }

    public override void TriggerInteractable()
    {
        ResetInteractable(); // makes sure each panel has full health.
        _collider.enabled = true;
        Debug.Log("triggered");
        psRef.SetActive(true);
        psRef.GetComponent<Animator>().SetTrigger("startVoidSurge");

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<Projectile>() != null)
        {

            Destroy(other.gameObject);

        }

    }


    public GameObject GetShieldPlayer()
    {
        //required for the VoidWasp Projectiles to make sure they aren't damaging their own player's shields.
        return _surgeObject;
    }

    public void Explode()
    {
        objectsHit = Physics.OverlapSphere(_immunePlayer.transform.localPosition, _explosionRadius);
        //Debug.Log("2 explosion radius: " + _explosionRadius);
        foreach (Collider obj in objectsHit)
        {
            Debug.Log("Object hit: " + obj.gameObject.name);
          //  if (obj.gameObject != _immunePlayer)
            //{
                if (obj.gameObject.GetComponent<CarHeatManager>() != null)
                {//if a car was hit
                    obj.gameObject.GetComponent<CarHeatManager>().AddHeat(_collectedDamage/4);
                    Debug.Log("1 damage done: " + _collectedDamage);
                }
                else if (obj.gameObject.GetComponent<Interactable>() != null)
                {
                    obj.gameObject.GetComponent<Interactable>().DamageInteractable(_collectedDamage/4);
                    Debug.Log("2 damage done: " + _collectedDamage);
                }

               // Debug.Log("3 damage done: " + _collectedDamage);
               // Debug.Log(obj.gameObject.name);
            //}

        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 *   Mike R.
 *   Eddie B.
 *   Document Created: 21/11/2019
 *
 *   The defensive ability for the void wasp.
 *   Activates a Sphere collider that collects all the damage done to the car and keeps track of it. 
 *   When over it, will release the damage in a AOE effect   
 **/

public class VoidWasp_ShieldBehaviour : Interactable
{
    [Header("Shield Settings")]
    [Tooltip("Reference to shield particle system")]
    public GameObject psRef;
    
    private float _maxHealth;
    private GameObject _surgeObject;
    private SphereCollider _collider;
    private MeshRenderer _meshRender;
    private float _collectedDamage;
    private GameObject _immunePlayer;

    //Collider[] objectsHit;

    [Header("Shield Explosion Settings")]
    [Tooltip("Sphere Collider that does damage to other objects when triggered")]
    public GameObject explosionDamageCollider;

    [Tooltip("List of all the parts from the particle explosion so we can trigger it")]
    public List<ParticleSystem> explosionParts;

    private float _explosionRadius;

    // Start is called before the first frame update
    void Start()
    {
        _collider = gameObject.GetComponent<SphereCollider>();
        _meshRender = gameObject.GetComponent<MeshRenderer>();
        canBeDamaged = true;
        psRef.SetActive(false);
        _meshRender.enabled = false;
        _collider.enabled = false;

        explosionDamageCollider.SetActive(false);
    }

    public void GiveInfo(float maxHealth, GameObject shieldRef, float explosionRadius, GameObject immunePlayer)
    {
        interactableHealth = maxHealth;
        _maxHealth = maxHealth;
        _surgeObject = shieldRef;
        _explosionRadius = explosionRadius;
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

        //print("shield health: " + interactableHealth);
    }

    public override void DestroyInteractable()
    {
        
        psRef.SetActive(false);
        _meshRender.enabled = false;
        _collider.enabled = false;

        Explode();
    }

    public override void ResetInteractable()
    {
        interactableHealth = _maxHealth;
        _collectedDamage = 0;
    }

    public override void TriggerInteractable()
    {
        ResetInteractable(); 
        _collider.enabled = true;
        psRef.SetActive(true);

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

        foreach (ParticleSystem part in explosionParts)
        {
            part.Play();
        }

        explosionDamageCollider.GetComponent<SphereCollider>().radius = _explosionRadius;
        explosionDamageCollider.SetActive(true);
        Invoke("TriggerExplosion", 0.5f);
    }

    void TriggerExplosion()
    {
        explosionDamageCollider.GetComponent<VoidWasp_Shield_Explosion>().DoDamage(_collectedDamage, _immunePlayer);
        explosionDamageCollider.SetActive(false);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lux_ShieldPanelBehavior : Interactable
{
    private float _shieldMaxHealth;
    private GameObject _shieldPlayer;
    private Collider _collider;
    public MeshRenderer _meshRender;

    private void Awake()
    {
        canBeDamaged = false;

        DestroyInteractable();
    }

    public void GiveInfo(float shieldHealth, GameObject shieldPlayer)
    {
        _shieldMaxHealth = shieldHealth;
        interactableHealth = shieldHealth;
        _shieldPlayer = shieldPlayer;
    }

    public override void TriggerInteractable()
    {
        ResetInteractable(); // makes sure each panel has full health.
    }

    public override void DestroyInteractable()
    {
        _collider = gameObject.GetComponent<Collider>();
        _meshRender = gameObject.GetComponent<MeshRenderer>();
        _meshRender.enabled = false;
        _collider.enabled = false;
    }

    public override void ResetInteractable()
    {
        _collider = gameObject.GetComponent<Collider>();
        _meshRender = gameObject.GetComponent<MeshRenderer>();
        _meshRender.enabled = true;
        _collider.enabled = true;
        interactableHealth = _shieldMaxHealth;
    }

    public override void DamageInteractable(float damageNumber)
    {
        if(canBeDamaged)
        {
            interactableHealth -= damageNumber;
            if (interactableHealth <= 0)
            {
                DestroyInteractable();
            }
        }

    }

    public GameObject GetShieldPlayer()
    {//required for the LuxProjectiles to make sure they aren't damaging their own player's shields.
        return _shieldPlayer;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Projectile"))
        {
            if (other.GetComponent<Projectile>().getImmunePlayer() != _shieldPlayer)
            {
               // Debug.Log(other.name + "should have been destroyed b");
                //other.GetComponent<Projectile>().SetProjectileDamage(0);
               Destroy(other);
            }

        }
    }

}

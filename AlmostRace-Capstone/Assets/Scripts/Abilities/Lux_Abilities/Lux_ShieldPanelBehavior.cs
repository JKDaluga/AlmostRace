using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lux_ShieldPanelBehavior : Interactable
{
    private float _shieldMaxHealth;
    private GameObject _shieldPlayer;
    private Collider _collider;
    private MeshRenderer _meshRender;

    private void Start()
    {
        canBeDamaged = true;
        _collider = gameObject.GetComponent<Collider>();
        _meshRender = gameObject.GetComponent<MeshRenderer>();
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
        _meshRender.enabled = false;
        _collider.enabled = false;
    }

    public override void ResetInteractable()
    {
        _meshRender.enabled = true;
        _collider.enabled = true;
        interactableHealth = _shieldMaxHealth;
    }

    public override void DamageInteractable(float damageNumber)
    {
        interactableHealth -= damageNumber;
        if (interactableHealth <= 0)
        {
            DestroyInteractable();
        }
    }

    public GameObject GetShieldPlayer()
    {//required for the LuxProjectiles to make sure they aren't damaging their own player's shields.
        return _shieldPlayer;
    }

    // _shieldHealth = _shieldHealthMax; add this later to where the shield dies

}

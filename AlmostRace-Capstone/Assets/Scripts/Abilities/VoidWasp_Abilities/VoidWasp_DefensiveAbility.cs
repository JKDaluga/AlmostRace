using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    Mike Romeo
    - Document Created: 21/11/2019

    Title:  Void Surge
    This script activates and deactivates the void wasp defensive ability.
    
*/

public class VoidWasp_DefensiveAbility : CooldownAbility
{

    [Tooltip("Set the max amount of damage the shield can absorb")]
    public float maxVoidHealth;

    [Tooltip("Reference to the void surge game object")]
    public GameObject surgeObject;

    [Tooltip("Set explosion radius")]
    public float explosionRadius;

    private GameObject _immunePlayer;

    void Start()
    {
        _immunePlayer = gameObject;
        surgeObject.GetComponent<VoidWasp_ShieldBehaviour>().GiveInfo(maxVoidHealth, gameObject, explosionRadius, _immunePlayer);
    }

    public override void ActivateAbility()
    {
        surgeObject.GetComponent<VoidWasp_ShieldBehaviour>().TriggerInteractable();
    }

    public override void DeactivateAbility()
    {
        surgeObject.GetComponent<VoidWasp_ShieldBehaviour>().DestroyInteractable();
    }

}

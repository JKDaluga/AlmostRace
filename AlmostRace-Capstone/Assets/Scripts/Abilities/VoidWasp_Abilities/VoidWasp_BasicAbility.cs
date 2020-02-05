using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Edouard Borissov 
 * This script handles the Void Wasp's basic ability that allows it to phase out of reality to damage
 * cars it passes through.
 * Abilities (such as rockets) will pass through it as well.
 * 
 * The void collider is a separate object
 */


public class VoidWasp_BasicAbility : HeatAbility
{
    private int _originalPhysicsLayer; // The physics layer that the car initially has.
    private int _voidStepPhysicsLayer = 10; // The physics layer that allows the car to pass through things

    [Header("Basic Ability Variables")]

    [Tooltip("The DPS of the ability.")]
    public float dps;

    [Tooltip("The frequency of the DPS of the ability.")]
    public float dpsFrequency;

    [Tooltip("The trigger collider that deals DPS.")]
    public Collider voidStepCollider;

    [Header("Basic Ability Aesthetics")]

    [Tooltip("The object that will be transparent.")] //Might make this a list, and change the alpha values through a For loop.
    public MeshRenderer mainVisibleObject;

    [Tooltip("The car's original material.")]
    public Material _originalMaterial;

    [Tooltip("The car's transparent material.")]
    public Material _transparentMaterial;

    void Start()
    {
        _originalPhysicsLayer = gameObject.layer;
        carHeatInfo = gameObject.GetComponent<CarHeatManager>();
        voidStepCollider.enabled = false;
    }

    public override void ActivateAbility()
    {
        if(gameObject.layer == _originalPhysicsLayer) // Stops the following from happening many times a frame.
        {
            gameObject.layer = _voidStepPhysicsLayer;
            mainVisibleObject.material = _transparentMaterial; // Might have to change to .materials[] if objects have more than 1 material.          
            InvokeRepeating("AddHeat", 0, dpsFrequency);
        }
        
    }

    public override void DeactivateAbility()
    {
        if(gameObject.layer == _voidStepPhysicsLayer)
        {    
            gameObject.layer = _originalPhysicsLayer;
            mainVisibleObject.material = _originalMaterial;
            CancelInvoke("AddHeat");
        }
   
    }

    protected override void AddHeat()
    {
        carHeatInfo.healthCurrent += selfHeatDamage;
    }
}

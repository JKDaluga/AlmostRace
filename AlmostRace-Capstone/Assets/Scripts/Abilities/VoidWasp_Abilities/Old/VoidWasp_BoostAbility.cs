using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/** 
 *  Mike R.
 *  Eddie B.
 *  Robin R.
 *  
 *  Increases speed and Top speed by a percentage. 
 *  When trigger it will switch the car to a different physics layer 
 *  so the var can phase trough objects and, changes the material to a transparent one.
 **/

public class VoidWasp_BoostAbility : CooldownHeatAbility
{

    [Header("Boost settings")]
    private SphereCarController carInfo;

    public GameObject carCollider;

    [Range(0, 100)]
    public float boostSpeedPercentage;

    [Range(0, 100)]
    public float boostTopSpeedPercentage;


    [Header("VoidWasp Boost Effects")]
    [Tooltip("Set Voidwasp booster particle effect")]
    public GameObject voidParticles;


    [Tooltip("List of VoisWasp parts that get reassigned on boost")]
    public List<GameObject> voidWaspParts;
    [Tooltip("Transparent material that needs to get assigned to every part")]
    public Material boostMat;

    private Material _originalMat;

    private void Start()
    {
        carInfo = gameObject.GetComponent<SphereCarController>();
        voidParticles.SetActive(false);

        _originalMat = voidWaspParts[0].GetComponent<Renderer>().material;

        // Set reference to Car Heat manager
        carHeatInfo = gameObject.GetComponent<CarHealthBehavior>();
    }

    public override void ActivateAbility()
    {
        carInfo.SetIsBoosting(true);
        carInfo.SetBoostInfo(boostSpeedPercentage);
        voidParticles.SetActive(true);

        SetMaterials(voidWaspParts, boostMat);

        gameObject.layer = 21;
        carCollider.layer = 21;

        AudioManager.instance.Play("VoidWasp Boost");

        AddHeat();
    }

    public override void DeactivateAbility()
    {
        carInfo.SetIsBoosting(false);
        voidParticles.SetActive(false);

        SetMaterials(voidWaspParts, _originalMat);

        AudioManager.instance.Play("VoidWasp Boost Down");

        gameObject.layer = 8;
        carCollider.layer = 9;

    }

    protected override void AddHeat()
    {
        carHeatInfo.DamageCar(selfHeatDamage);
    }

    void SetMaterials(List<GameObject> parts, Material mat)
    {
        foreach (GameObject part in parts)
        {
            part.GetComponent<Renderer>().material = mat;
        }
    }

   
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 * Eddie B. && Leo C.
 * This script handles the Juggernaut's Drill basic ability.
 */
public class Juggernaut_BasicAbility : BasicAbility
{
    [Header("Movement Variables Affected")]
    public SphereCarController sphereCarScript;
    private float _originalTopSpeed;
    private float _originalAcceleration;
    private float _originalSteering;

    public float drillNewTopSpeed;
    public float drillNewAcceleration;
    public float drillNewSteering;

    [Header("Drill Damage Variables")]
    public Juggernaut_Drill drillScript;

    public float drillDamage;
    public float drillFrequency;
    public float selfDamageFrequency;
    public float hypeGained;

    private GameObject _immunePlayer;
 

    public void Start()
    {
        base.Initialize();
        SetDrillMovementInfo();
        carHeatInfo = gameObject.GetComponent<CarHeatManager>();
        drillScript.SetDrillInfo(drillDamage, drillFrequency, hypeGained, vehicleHypeScript, _immunePlayer);
    }

    private void SetDrillMovementInfo()
    {
        _originalTopSpeed = sphereCarScript.topSpeed;
        _originalAcceleration = sphereCarScript.acceleration;
        _originalSteering = sphereCarScript.steering;
    }

    private void ActivateDrillMovement()
    {
        sphereCarScript.topSpeed = drillNewTopSpeed;
        sphereCarScript.acceleration = drillNewAcceleration;
        sphereCarScript.steering = drillNewSteering;
    }

    private void DeactivateDrillMovement()
    {
        sphereCarScript.topSpeed = _originalTopSpeed;
        sphereCarScript.acceleration = _originalAcceleration;
        sphereCarScript.steering = _originalSteering;
    }

    public override void ActivateAbility()
    {
        if (!drillScript.GettIsSpinning())
        {
            ActivateDrillMovement();
            InvokeRepeating("AddHeat", 0, selfDamageFrequency);
            drillScript.SetIsSpinning(true);
            drillScript.StartSpinVisuals(); 
        }          
    }

    public override void DeactivateAbility()
    {
        if (drillScript.GettIsSpinning())
        {
            DeactivateDrillMovement();
            CancelInvoke("AddHeat");
            drillScript.SetIsSpinning(false);
            drillScript.StopSpinVisuals();
        }
    }

    protected override void AddHeat()
    {
        carHeatInfo.heatCurrent += selfHeatDamage;
    }

}

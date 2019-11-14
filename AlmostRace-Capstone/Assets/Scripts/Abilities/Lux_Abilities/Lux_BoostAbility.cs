using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lux_BoostAbility : CooldownHeatAbility
{
    private SphereCarController carInfo;

    [Range(0, 100)]
    public float boostSpeedPercentage;

    [Range(0, 100)]
    public float boostTopSpeedPercentage;

    [Header("Jet engine effect")]
    [Tooltip("Set Lux booster particle effect")]
    public ParticleSystem jetParticles;

    [Tooltip("Set the amount boosting increases the jet length")]
    [Range(2, 10)]
    public float jetIncrease;

    // Save original original jet speed setting
    private ParticleSystem.MinMaxCurve _originalJetSpeed;

    private void Start()
    {
        carInfo = gameObject.GetComponent<SphereCarController>();   
    }

    public override void ActivateAbility()
    {
        carInfo.SetIsBoosting(true);
        carInfo.SetBoostInfo(boostSpeedPercentage);
        
        // Set the new jet particle speed
        var jpMain = jetParticles.main;
        // Save the original before updating the speed
        _originalJetSpeed = jpMain.startSpeed;
        // Change the speed while boosting
        jpMain.startSpeed = jetIncrease;

        AddHeat();
    }

    public override void DeactivateAbility()
    {
        // Reset the jet speed when you stop boosting
        var jpMain = jetParticles.main;
        jpMain.startSpeed = _originalJetSpeed;

        carInfo.SetIsBoosting(false);

       
    }

    protected override void AddHeat()
    {
      // carHeatInfo.AddHeat(selfHeatDamage);
    }
}

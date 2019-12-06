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
    public ParticleSystem jetParticles2;
    public ParticleSystem jetParticles3;

    [Tooltip("Set the amount boosting increases the jet length")]
    [Range(2, 10)]
    public float jetIncrease;

    // Save original original jet speed setting
    private ParticleSystem.MinMaxCurve _originalJetSpeed;
    private ParticleSystem.MinMaxCurve _originalJetSpeed2;

    private void Start()
    {
        carInfo = gameObject.GetComponent<SphereCarController>();
        carHeatInfo = gameObject.GetComponent<CarHeatManager>();
    }

    public override void ActivateAbility()
    {
        carInfo.SetIsBoosting(true);
        carInfo.SetBoostInfo(boostSpeedPercentage);
        
        // Set the new jet particle speed
        var jpMain = jetParticles.main;
        var jpMain2 = jetParticles2.main;
        var jpMain3 = jetParticles3.main;
        // Save the original before updating the speed
        _originalJetSpeed = jpMain.startSpeed;
        _originalJetSpeed2 = jpMain2.startSpeed;
        // Change the speed while boosting
        jpMain.startSpeed = jetIncrease;
        jpMain2.startSpeed = jetIncrease;
        jpMain3.startSpeed = jetIncrease;

        AddHeat();
    }

    public override void DeactivateAbility()
    {
        // Reset the jet speed when you stop boosting
        var jpMain = jetParticles.main;
        var jpMain2 = jetParticles2.main;
        var jpMain3 = jetParticles3.main;

        jpMain.startSpeed = _originalJetSpeed;
        jpMain2.startSpeed = _originalJetSpeed2;
        jpMain3.startSpeed = _originalJetSpeed2;

        carInfo.SetIsBoosting(false);
    }

    protected override void AddHeat()
    {
       carHeatInfo.AddHeat(selfHeatDamage);
    }
}

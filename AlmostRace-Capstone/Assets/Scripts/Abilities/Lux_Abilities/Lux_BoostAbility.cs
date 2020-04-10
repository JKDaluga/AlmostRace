using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 *  Eddie B.
 *  Mike R.
 *  
 *  When triggered, increases top Speed settings and adjusts 
 *  the jet enginge particle effects length. 
 **/

public class Lux_BoostAbility : CooldownHeatAbility
{
    private RaycastCar carInfo;
    private bool isBoosting = false;

    [Header("Movement Values")]
    [Range(0, 100)]
    [Tooltip("The percentage speed increase added to the top speed while boosting.")]
    public float boostSpeedPercentage;

    [Tooltip("The percentage of the original maxTurnAngle the car is allowed during a boost.")]
    [Range(0, 100)]
    public float maxBoostTurnAngle;
    private float originalMaxTurnAngle;
   //[Range(0, 100)]
   //public float boostTopSpeedPercentage;

    [Header("Visuals: Jet engine effect")]
    [Tooltip("Set Lux booster particle effect")]
    public ParticleSystem jetParticles;
    public ParticleSystem jetParticles2;
    public ParticleSystem jetParticles3;

    public bool shouldAnimate = true;
    public Animator leftWingAnimator;
    public Animator rightWingAnimator;
    public GameObject cooldownPulseVFXLeft;
    public GameObject cooldownPulseVFXRight;
    public List<GameObject> vfxToActivate = new List<GameObject>();

    [Tooltip("Set the amount boosting increases the jet length")]
    [Range(2, 10)]
    public float jetIncrease;

    // Save original original jet speed setting
    private ParticleSystem.MinMaxCurve _originalJetSpeed;
    private ParticleSystem.MinMaxCurve _originalJetSpeed2;

    private void Start()
    {
        carInfo = gameObject.GetComponent<RaycastCar>();
        carHeatInfo = gameObject.GetComponent<CarHealthBehavior>();
        originalMaxTurnAngle = carInfo.maxTurnAngle;

    }

    public override void ActivateAbility()
    {
       if(!isBoosting)
        {
            isBoosting = true;
            carInfo.setBoostSpeed(boostSpeedPercentage / 100);
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
            carInfo.maxTurnAngle = originalMaxTurnAngle * (maxBoostTurnAngle / 100);
            //AddHeat();
        }
       
       //carInfo.SetBoostInfo(boostSpeedPercentage);              
    }

    public override void DeactivateAbility()
    {
        // Reset the jet speed when you stop boosting
        carInfo.ResetBoostSpeed();
        var jpMain = jetParticles.main;
        var jpMain2 = jetParticles2.main;
        var jpMain3 = jetParticles3.main;

        jpMain.startSpeed = _originalJetSpeed;
        jpMain2.startSpeed = _originalJetSpeed2;
        jpMain3.startSpeed = _originalJetSpeed2;

        isBoosting = false;
        carInfo.maxTurnAngle = originalMaxTurnAngle;
    }

    protected override void AddHeat()
    {
      // carHeatInfo.DamageCarTrue(selfHeatDamage);
    }


    public override void AbilityOnCooldown()
    {
        if (shouldAnimate)
        {
          
            foreach (GameObject vfx in vfxToActivate)
            {
                vfx.SetActive(false);
            }
        }
    }

    public override void AbilityOffOfCooldown()
    {
        if(shouldAnimate)
        {
            cooldownPulseVFXLeft.SetActive(false);
            cooldownPulseVFXRight.SetActive(false);
            leftWingAnimator.SetTrigger("WingDownLeft");
            rightWingAnimator.SetTrigger("WingDownRight");

        }

    }

    public override void AbilityInUse()
    {
        if (shouldAnimate)
        {
            cooldownPulseVFXLeft.SetActive(true);
            cooldownPulseVFXRight.SetActive(true);
            leftWingAnimator.SetTrigger("WingUpLeft");
            rightWingAnimator.SetTrigger("WingUpRight");
            foreach (GameObject vfx in vfxToActivate)
            {
                vfx.SetActive(true);
            }
        }
    }
}

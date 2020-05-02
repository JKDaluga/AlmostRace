using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Eddie and Greg

public class PainTrain_Boost : CooldownHeatAbility
{

    private RaycastCar carInfo;
    private bool isBoosting = false;
    public GameObject boostField;
    private PainTrain_BoostShock _boostFieldScript;

    [Header("Movement Values")]

    [Tooltip("The percentage speed increase added to the top speed while boosting.")]
    [Range(0, 100)]
    public float boostSpeedPercentage;

    [Tooltip("The percentage of the original maxTurnAngle the car is allowed during a boost.")]
    [Range(0, 100)]
    public float maxBoostTurnAngle;

    private float originalMaxTurnAngle;

    [Tooltip("How often the boosts AOE damage happens each second. So a value of 2 means every 2 seconds, a value of .5 means every half second.")]
    public float boostDamageRate;
    [Tooltip("How much damage the AOE does")]
    public float boostDamage;
    
    [Header("Visuals")]
    public float smallSize = 2f;
    public float mediumSize = 4;
    public float bigSize = 8;

    public List<GameObject> vfxToActivate = new List<GameObject>();
    public List<ParticleSystem> boosterParticles = new List<ParticleSystem>();



    // Start is called before the first frame update
    void Start()
    {
        _boostFieldScript = boostField.GetComponent<PainTrain_BoostShock>();
        _boostFieldScript.GiveInfo(gameObject, boostDamage, boostDamageRate);

        carInfo = gameObject.GetComponent<RaycastCar>();
        AbilityOffOfCooldown();
        if (carInfo != null)
        {
            originalMaxTurnAngle = carInfo.maxTurnAngle;
        }
    }

    public override void ActivateAbility()
    {
        AudioManager.instance.Play("Pain Train Boost", transform);
        if (!isBoosting)
        {
            isBoosting = true;
            if (carInfo != null)
            {
                carInfo.setBoostSpeed(boostSpeedPercentage / 100);
                carInfo.maxTurnAngle = originalMaxTurnAngle * (maxBoostTurnAngle / 100);
            }
            boostField.SetActive(true);
        }

        foreach(GameObject vfxObj in vfxToActivate)
        {
            vfxObj.SetActive(true);
        }
    }

    public override void DeactivateAbility()
    {
        if (carInfo != null)
        {
            carInfo.ResetBoostSpeed();
            carInfo.maxTurnAngle = originalMaxTurnAngle;
            boostField.GetComponent<PainTrain_BoostShock>().ClearCarList();
        }
        boostField.SetActive(false);
        isBoosting = false;

        foreach (GameObject vfxObj in vfxToActivate)
        {
            vfxObj.SetActive(false);
        }

    }

    protected override void AddHeat()
    {
        //nah
    }



    public override void AbilityOnCooldown()
    {
        foreach (ParticleSystem ps in boosterParticles)
        {
            var psMain = ps.main;
            psMain.startSize = smallSize;
        }
    }

    public override void AbilityOffOfCooldown()
    {
        foreach (ParticleSystem ps in boosterParticles)
        {
            var psMain = ps.main;
            psMain.startSize = mediumSize;
        }
    }

    public override void AbilityInUse()
    {
        foreach (ParticleSystem ps in boosterParticles)
        {
            var psMain = ps.main;
            psMain.startSize = bigSize;
        }
    }
}

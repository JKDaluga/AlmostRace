using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PainTrain_Boost : CooldownHeatAbility
{

    private RaycastCar carInfo;
    private bool isBoosting = false;

    [Range(0, 1)]
    public float boostSpeedPercentage;

    // Start is called before the first frame update
    void Start()
    {
        carInfo = gameObject.GetComponent<RaycastCar>();
        carHeatInfo = gameObject.GetComponent<CarHealthBehavior>();
    }

    public override void ActivateAbility()
    {
        if (!isBoosting)
        {
            isBoosting = true;
            carInfo.setBoostSpeed(boostSpeedPercentage);

            AddHeat();
        }
    }

    public override void DeactivateAbility()
    {
        carInfo.setBoostSpeed(0);

        isBoosting = false;
    }

    protected override void AddHeat()
    {
        carHeatInfo.DamageCarTrue(selfHeatDamage);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lux_BoostAbility : CooldownHeatAbility
{
    private SphereCarController carInfo;

    [Range(0,100)]
    public float boostSpeedPercentage;

    [Range(0, 100)]
    public float boostTopSpeedPercentage;

    private void Start()
    {
        carInfo = gameObject.GetComponent<SphereCarController>();   
    }

    public override void ActivateAbility()
    {
        carInfo.SetIsBoosting(true);
        carInfo.SetBoostInfo(boostSpeedPercentage);
        AddHeat();
    }

    public override void DeactivateAbility()
    {
        carInfo.SetIsBoosting(false);
    }

    protected override void AddHeat()
    {
      // carHeatInfo.AddHeat(selfHeatDamage);
    }
}

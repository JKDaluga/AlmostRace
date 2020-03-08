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
    public float boostDamageRate;
    public float boostDamage;

    [Range(0, 1)]
    public float boostSpeedPercentage;

    // Start is called before the first frame update
    void Start()
    {
        _boostFieldScript = boostField.GetComponent<PainTrain_BoostShock>();

        _boostFieldScript.GiveInfo(gameObject, boostDamage, boostDamageRate);

        carInfo = gameObject.GetComponent<RaycastCar>();
    }

    public override void ActivateAbility()
    {
        if (!isBoosting)
        {
            isBoosting = true;
            carInfo.setBoostSpeed(boostSpeedPercentage);

            boostField.SetActive(true);
        }
    }

    public override void DeactivateAbility()
    {
        carInfo.setBoostSpeed(0);
        boostField.GetComponent<PainTrain_BoostShock>().ClearCarList();
        boostField.SetActive(false);

        isBoosting = false;

    }

    protected override void AddHeat()
    {
        //nah
    }
}

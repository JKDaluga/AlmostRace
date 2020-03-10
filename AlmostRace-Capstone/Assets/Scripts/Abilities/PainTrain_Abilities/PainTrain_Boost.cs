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

    public float smallSize = 2f;
    public float mediumSize = 4;
    public float bigSize = 8;

    public List<GameObject> vfxToActivate = new List<GameObject>();
    public List<ParticleSystem> boosterParticles = new List<ParticleSystem>();

    [Range(0, 1)]
    public float boostSpeedPercentage;

    // Start is called before the first frame update
    void Start()
    {
        _boostFieldScript = boostField.GetComponent<PainTrain_BoostShock>();

        _boostFieldScript.GiveInfo(gameObject, boostDamage, boostDamageRate);

        carInfo = gameObject.GetComponent<RaycastCar>();
        AbilityOffOfCooldown();
    }

    public override void ActivateAbility()
    {
        if (!isBoosting)
        {
            isBoosting = true;
            carInfo.setBoostSpeed(boostSpeedPercentage);

            boostField.SetActive(true);
        }
        foreach(GameObject vfxObj in vfxToActivate)
        {
            vfxObj.SetActive(true);
        }
    }

    public override void DeactivateAbility()
    {
        carInfo.setBoostSpeed(0);
        boostField.GetComponent<PainTrain_BoostShock>().ClearCarList();
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

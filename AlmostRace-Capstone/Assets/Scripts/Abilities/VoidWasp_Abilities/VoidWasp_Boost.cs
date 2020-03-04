using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    /*
    Author: Jake Velicer
    Purpose: Contains the behavior for the Void Wasp boost.
    In particular, when this script's activate ability is called, the boost is started.
    Companions are turned on along side the Void Wasp. As the vehicle takes damage,
    the boost percentage applied is lessoned and companions get turned off.
    */

public class VoidWasp_Boost : CooldownHeatAbility
{
    [Range(0, 1)] public float boostSpeedPercentage;
    public float healthLossActivateAmount = 25;
    private float currentBoostPercentage;
    public GameObject[] companions;
    public GameObject explodeVFX;
    private RaycastCar carInfo;
    private bool isBoosting = false;

    private void Start()
    {
        carInfo = gameObject.GetComponent<RaycastCar>();
        carHeatInfo = gameObject.GetComponent<CarHealthBehavior>();
    }

    public override void ActivateAbility()
    {
        if(!isBoosting)
        {
            isBoosting = true;
            currentBoostPercentage = boostSpeedPercentage;
            carInfo.setBoostSpeed(currentBoostPercentage);
            for (int i = 0; i < companions.Length; i++)
            {
                companions[i].SetActive(true);
            }
            StartCoroutine(CompanionBehavior());
        }
    }

    private IEnumerator CompanionBehavior()
    {
        float startHealth = carHeatInfo.healthCurrent;
        float lastHealth = startHealth;
        float difference = 0;
        
        while(true)
        {
            startHealth = carHeatInfo.healthCurrent;
            float healthToAdd = lastHealth - startHealth;
            difference = Mathf.Abs(difference += healthToAdd);
            lastHealth = startHealth;
            
            if (difference > healthLossActivateAmount)
            {
                currentBoostPercentage = currentBoostPercentage * 0.75f;
                carInfo.setBoostSpeed(currentBoostPercentage);
                difference = 0;

                for (int i = companions.Length - 1; i > 0; i--)
                {
                    if(companions[i].activeSelf)
                    {
                        companions[i].SetActive(false);
                        Instantiate(explodeVFX, companions[i].transform.position, companions[i].transform.rotation);
                        AudioManager.instance.Play("VoidWasp Companion Death", transform);
                        break;
                    }
                }
            }
            if (currentBoostPercentage <= boostSpeedPercentage * 0.50f)
            {
                DeactivateAbility();
            }
            yield return null;
        }
    }

    public override void DeactivateAbility()
    {
        StopAllCoroutines();
        carInfo.setBoostSpeed(0);
        for (int i = 0; i < companions.Length; i++)
        {
            companions[i].SetActive(false);
            Instantiate(explodeVFX, companions[i].transform.position, companions[i].transform.rotation);
            AudioManager.instance.Play("VoidWasp Companion Death", transform);
        }
        isBoosting = false;
    }

    protected override void AddHeat() {}

}

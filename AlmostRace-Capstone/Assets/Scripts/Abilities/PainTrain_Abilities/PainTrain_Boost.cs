using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PainTrain_Boost : CooldownHeatAbility
{

    private RaycastCar carInfo;
    private bool isBoosting = false;
    private CarHealthBehavior _immunePlayer;
    public GameObject boostField;
    public float _boostDamageRate;
    public float _boostDamage;

    private Interactable hitInteractable;

    private List<CarHealthBehavior> _damagedCars;

    [Range(0, 1)]
    public float boostSpeedPercentage;

    // Start is called before the first frame update
    void Start()
    {
        _damagedCars = new List<CarHealthBehavior>();
        carInfo = gameObject.GetComponent<RaycastCar>();
        carHeatInfo = gameObject.GetComponent<CarHealthBehavior>();
        _immunePlayer = gameObject.GetComponent<CarHealthBehavior>();
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

    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<CarHealthBehavior>()!=null && other.GetComponent<RaycastCar>() != carInfo)
        {
            _damagedCars.Add(other.GetComponent<CarHealthBehavior>());

            if (_damagedCars.Count == 1)
            {
                InvokeRepeating("DamageCars", 0, _boostDamageRate);
            }
        }
        else if (other.gameObject.GetComponent<Interactable>() != null)
        {
            hitInteractable = other.gameObject.GetComponent<Interactable>();
            hitInteractable.DamageInteractable(hitInteractable.interactableHealth);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<CarHealthBehavior>() != null && _damagedCars.Contains(other.gameObject.GetComponent<CarHealthBehavior>()))
        {
            _damagedCars.Remove(other.gameObject.GetComponent<CarHealthBehavior>());
        }
    }

    void DamageCars()
    {
        if (_damagedCars.Count != 0)
        {
            foreach (CarHealthBehavior car in _damagedCars)
            {
                if (!car.isDead)
                {
                    car.DamageCar(_boostDamage);
                    
                        if (car.healthCurrent <= 0)
                        {
                            _damagedCars.Remove(car);
                        }
                }
            }
        }
        else
        {
            CancelInvoke("DamageCars");
        }
    }

    public override void DeactivateAbility()
    {
        carInfo.setBoostSpeed(0);

        boostField.SetActive(false);

        isBoosting = false;
    }

    protected override void AddHeat()
    {
    }
}

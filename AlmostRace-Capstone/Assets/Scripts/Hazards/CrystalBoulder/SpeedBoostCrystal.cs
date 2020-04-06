using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedBoostCrystal : MonoBehaviour
{
    public float speedBoostPercentage;
    public bool isActive = false;
    private List<RaycastCar> _boostedCars = new List<RaycastCar>();
    private RaycastCar _carToAdd;

    public void ActivateCrystal()
    {
        isActive = true;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (isActive)
        {
            if (other.gameObject.GetComponent<RaycastCar>() != null)
            {
                _carToAdd = other.gameObject.GetComponent<RaycastCar>();
                if (!_boostedCars.Contains(_carToAdd))
                {
                    _boostedCars.Add(_carToAdd);
                    other.gameObject.GetComponent<RaycastCar>().SetBoostPadSpeed(speedBoostPercentage / 100);
                    //   StartCoroutine(ResetBoost(boostTime, _carToAdd));
                }

            }
        }

    }

    public IEnumerator ResetBoost(float timeToReset, RaycastCar carToReset)
    {
        yield return new WaitForSeconds(timeToReset);

        carToReset.gameObject.GetComponent<RaycastCar>().ResetBoostPadSpeed();
        _boostedCars.Remove(carToReset);
    }
}

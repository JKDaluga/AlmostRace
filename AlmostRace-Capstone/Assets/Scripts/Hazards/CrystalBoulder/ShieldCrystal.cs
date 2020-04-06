using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldCrystal : MonoBehaviour
{
    public float shieldGainAmount;
    public float shieldDuration = 3f;
    public bool isActive = false;
    private List<CarHealthBehavior> _shieldedCars = new List<CarHealthBehavior>();
    private CarHealthBehavior _carToAdd;

    public void ActivateCrystal()
    {
        isActive = true;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (isActive)
        {
            if (other.gameObject.GetComponent<CarHealthBehavior>() != null)
            {
                _carToAdd = other.gameObject.GetComponent<CarHealthBehavior>();
                if (!_shieldedCars.Contains(_carToAdd))
                {
                    _shieldedCars.Add(_carToAdd);
                    _carToAdd.AddExtraShields(shieldGainAmount);
                    StartCoroutine(ResetShield(shieldDuration, _carToAdd));
                }

            }
        }

    }

    public IEnumerator ResetShield(float timeToReset, CarHealthBehavior carToReset)
    {
        yield return new WaitForSeconds(timeToReset);
        carToReset.RemoveExtraShields(shieldGainAmount);
        _shieldedCars.Remove(carToReset);
    }
}

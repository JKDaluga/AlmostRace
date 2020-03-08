using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Eddie B

public class ShockRodSphere : MonoBehaviour
{
   // private GameObject _immunePlayer;

    private float _shockDamage;

    private float _shockRate;

    private List<CarHealthBehavior> _carsToDamage = new List<CarHealthBehavior>();

    public void GiveInfo( float shockDamage, float shockRate)
    {
       // _immunePlayer = immunePlayer;
        _shockDamage = shockDamage;
        _shockRate = shockRate;
    }

    public void OnDisable()
    {
        ClearCarList();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Vehicle"))
        {
            _carsToDamage.Add(other.GetComponent<CarHealthBehavior>());

            if (_carsToDamage.Count == 1)
            {
                InvokeRepeating("DamageCars", 0, _shockRate);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<CarHealthBehavior>() != null && _carsToDamage.Contains(other.gameObject.GetComponent<CarHealthBehavior>()))
        {
            _carsToDamage.Remove(other.gameObject.GetComponent<CarHealthBehavior>());
        }
    }


    public void ClearCarList()
    {
        _carsToDamage.Clear();
        CancelInvoke("DamageCars");
    }

    void DamageCars()
    {
        if (_carsToDamage.Count != 0)
        {
            foreach (CarHealthBehavior car in _carsToDamage)
            {
                if (!car.isDead)
                {
                    car.DamageCar(_shockDamage);
                    if (car.healthCurrent <= 0)
                    {
                        _carsToDamage.Remove(car);
                    }
                }
            }
        }
        else
        {
            ClearCarList();
        }
    }

}

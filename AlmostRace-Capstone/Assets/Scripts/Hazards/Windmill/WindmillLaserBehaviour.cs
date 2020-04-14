using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindmillLaserBehaviour : MonoBehaviour
{


    private float _laserDamage,_laserDamageRate;

    private GameObject _interactingPlayer;

    private List<CarHealthBehavior> _damagedCars;

    private void Start()
    {
        _damagedCars = new List<CarHealthBehavior>();
    }


    public void UpdateLaserDamage(float laserDmg)
    {
        _laserDamage = laserDmg;
    }

    public void UpdateDamageRate(float laserDmgRate)
    {
        _laserDamageRate = laserDmgRate;
    }

    private void OnTriggerEnter(Collider other)
    {
        
        if (other.gameObject.GetComponent<CarHealthBehavior>() != null)
        {
            _damagedCars.Add(other.gameObject.GetComponent<CarHealthBehavior>()); 
            if (_damagedCars.Count == 1)
            {
                InvokeRepeating("DamageCars", 0, _laserDamageRate);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<CarHealthBehavior>() != null && _damagedCars.Contains(other.gameObject.GetComponent<CarHealthBehavior>()))
        {
            _damagedCars.Remove(other.gameObject.GetComponent<CarHealthBehavior>());
        }
    }

    private void DamageCars()
    {
        CarHealthBehavior car;

        if (_damagedCars.Count != 0)
        {
            for(int i = 0; i < _damagedCars.Count; i++)
            {
                car = _damagedCars[i];
                if (!car.isDead)
                {
                   
                    
                    if (_interactingPlayer != null)
                    {
                        if (!_interactingPlayer.Equals(car.gameObject))
                        {
                            car.DamageCar(_laserDamage, _interactingPlayer.GetComponent<CarHealthBehavior>().raycastCarHolder.playerID);
                        }
                        if(car.healthCurrent <= 0)
                        {
                            _damagedCars.Remove(car);
                        }
                    }
                    else
                    {
                        car.DamageCar(_laserDamage, 100);
                        if (car.healthCurrent <= 0)
                        {
                            _damagedCars.Remove(car);
                        }
                    }
                }
            }
        }
        else
        {
            CancelInvoke("DamageCars");
        }
    }
}

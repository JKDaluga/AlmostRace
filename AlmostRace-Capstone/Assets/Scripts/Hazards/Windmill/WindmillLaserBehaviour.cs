using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindmillLaserBehaviour : MonoBehaviour
{

    private float _laserSpeed;
    private float _speedUpFactor = 1;
    private float _laserDamage,_laserDamageRate;

    private GameObject _interactingPlayer;

    private List<CarHealthBehavior> _damagedCars;

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, Mathf.Lerp(transform.localEulerAngles.z,transform.localEulerAngles.z+_laserSpeed,_speedUpFactor));
    }


    public void UpdateBaseSpeed(float baseSpeed)
    {
        _laserSpeed = baseSpeed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<CarHealthBehavior>() != null)
        {
            _damagedCars.Add(other.gameObject.GetComponent<CarHealthBehavior>()); //Adds car to damage.
            if (_damagedCars.Count == 1)
            {
                InvokeRepeating("DamageCars", 0, _laserDamageRate);
            }
        }
    }

    private void DamageCars()
    {
        if (_damagedCars.Count != 0)
        {
            foreach(CarHealthBehavior car in _damagedCars)
            {
                if (!car.isDead)
                {
                    car.DamageCar(_laserDamage);
                    if (_interactingPlayer != null)
                    {
                        if (!_interactingPlayer.Equals(car.gameObject))
                        {

                        }
                        if(car.healthCurrent <= 0)
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

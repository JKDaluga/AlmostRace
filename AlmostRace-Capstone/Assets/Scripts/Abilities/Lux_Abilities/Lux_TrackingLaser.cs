using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lux_TrackingLaser : MonoBehaviour
{

    private GameObject _target;

    float _laserDamageRate,_laserDamage;
    float _laserSpeed;

    private List<CarHealthBehavior> _damagedCars;

    // Start is called before the first frame update
    void Start()
    {
        _damagedCars = new List<CarHealthBehavior>();
        Destroy(gameObject, 10);
        _target = Lux_TrackingDart.FindObjectOfType<Lux_TrackingDart>()._hitObject;
        _laserDamageRate = Lux_TrackingDart.FindObjectOfType<Lux_TrackingDart>().laserDamageRate;
        _laserDamage = Lux_TrackingDart.FindObjectOfType<Lux_TrackingDart>().laserDamage;
        _laserSpeed = Lux_TrackingDart.FindObjectOfType<Lux_TrackingDart>().laserDmgSpeed;

        gameObject.transform.position = _target.transform.position;
    }

    public void GiveInfo()
    {

    }

    public void SetTarget(GameObject target)
    {
        _target = target;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        gameObject.transform.LookAt(_target.transform.position);

        gameObject.transform.position += transform.forward * _laserSpeed * Time.deltaTime;
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
        else if (other.gameObject.GetComponent<Interactable>() != null)
        {
            other.gameObject.GetComponent<Interactable>().DamageInteractable(_laserDamage);
            Destroy(gameObject);
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
        if (_damagedCars.Count != 0)
        {
            foreach (CarHealthBehavior car in _damagedCars)
            {
                if (!car.isDead)
                {
                    car.DamageCar(_laserDamage);

                    if (_target != null)
                    {
                        if (!_target.Equals(car.gameObject))
                        {

                        }
                        if (car.healthCurrent <= 0)
                        {
                            _damagedCars.Remove(car);
                            Destroy(gameObject);
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

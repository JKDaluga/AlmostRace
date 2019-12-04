using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoidWasp_ProjectileBehaviour : Projectile
{

    private float _speedIncrease;

    private float _speedRate;

    private float _speedLimit;

    private float _speedLimitActual;

    private new void Start()
    {
        base.Start();
        GiveSpeed();
        /*_collider = gameObject.GetComponent<Collider>();
        _rigidBody = gameObject.GetComponent<Rigidbody>();
        if (_immunePlayer != null && _immunePlayerScript != null)
        {
            _immunePlayerVelocity = _immunePlayer.gameObject.GetComponent<SphereCarController>().sphere.velocity;
            _rigidBody.velocity = _immunePlayerVelocity;

            _speedLimitActual = _rigidBody.velocity.magnitude + _speedLimit;
            InvokeRepeating("IncreaseSpeed", 1, _speedRate);
        }
        else
        {
            _rigidBody.velocity = transform.TransformDirection(Vector3.forward * _projectileSpeed);
        }*/
    }

    public void IncreaseSpeed()
    {
        _rigidBody.velocity += transform.TransformDirection(0,0, _speedIncrease);
        if (_rigidBody.velocity.z >= _speedLimitActual)
        {
            CancelInvoke();
        }
    }

    public void GiveInfo(float speedIncrease, float speedRate, float speedLimit)
    {
        _speedIncrease = speedIncrease;
        _speedRate = speedRate;
        _speedLimit = speedLimit;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (_isAlive)
        {
            if (other.gameObject != _immunePlayer && other.gameObject.GetComponent<CarHeatManager>() != null)
            {//Checks if the object isn't the immunePlayer and if they are a car.
                other.gameObject.GetComponent<CarHeatManager>().AddHeat(_projectileDamage);
                _immunePlayerScript.AddHype(_projectileHype, "Damage:");
                StartCoroutine(ExplosionEffect());
            }
            else if (other.gameObject != _immunePlayer && other.gameObject.GetComponent<Interactable>() != null)
            {//Checks if the object isn't the immunePlayer and if they are an interactable object.
                if (other.gameObject.GetComponent<VoidWasp_ShieldBehaviour>() != null)
                {//checks if about to hit a shield
                    if (other.gameObject.GetComponent<VoidWasp_ShieldBehaviour>().GetShieldPlayer() == _immunePlayer)
                    {
                        return;
                    }
                }

                other.gameObject.GetComponent<Interactable>().interactingPlayer = _immunePlayer;
                other.gameObject.GetComponent<Interactable>().DamageInteractable(_projectileDamage);
                StartCoroutine(ExplosionEffect());
            }
            else if (other.gameObject != _immunePlayer)
            {
                Debug.Log(gameObject.name + " has collided with: " + other.gameObject.name);
                StartCoroutine(ExplosionEffect());
            }
        }
    }
}

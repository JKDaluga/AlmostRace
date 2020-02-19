using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    /*
    Author: Jake Velicer
    Purpose: AMoming missile for the VoidWasp. This script takes in variables from the corresponding VoidWasp Attack Script.
    The missile then moves forward and toward its given target.
    */

public class VoidWasp_HomingMissile : Projectile
{
    private GameObject _target;
    private Quaternion _missileTargetRoation;
    private float _turnRate;
    private float _hangTime;
    private bool _canTrack;

    private void Start()
    {
        StartCoroutine(hangTimeSequence());
    }

    public void SetAdditionalInfo(GameObject giventTarget, float givenTurnRate, float givenHangTime)
    {
        _target = giventTarget;
        _turnRate = givenTurnRate;
        _hangTime = givenHangTime;
    }

    private void OnTriggerEnter(Collider collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            Destroy(gameObject);
        }
        if(collision.gameObject.GetComponent<CarHealthBehavior>() != null)
        {
            if(collision.gameObject != _immunePlayer)
            {
                collision.gameObject.GetComponent<CarHealthBehavior>().DamageCar(_projectileDamage);
                Destroy(gameObject);
            }
        }
        if(collision.gameObject.GetComponent<Interactable>() != null)
        {
            collision.gameObject.GetComponent<Interactable>().DamageInteractable(_projectileDamage);
            Destroy(gameObject);
        }
    }

    private IEnumerator hangTimeSequence()
    {
        yield return new WaitForSeconds(_hangTime);
        _canTrack = true;
    }

    private void FixedUpdate()
    {
        _rigidBody.velocity = transform.forward * _projectileSpeed;
        if (_canTrack)
        {
            _missileTargetRoation = Quaternion.LookRotation(_target.transform.position - transform.position);
            _rigidBody.MoveRotation(Quaternion.RotateTowards(transform.rotation, _missileTargetRoation, _turnRate));
        }
    }
}

﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    /*
    Author: Jake Velicer
    Purpose: AMoming missile for the VoidWasp. This script takes in variables from the corresponding VoidWasp Attack Script.
    The missile then moves forward and toward its given target.
    */

public class VoidWasp_HomingMissile : Projectile
{
    public GameObject explodeVFX;
    private GameObject _target;
    private Quaternion _missileTargetRotation;
    private CarHealthBehavior _carHealthStatus;
    private Interactable _interactableStatus;
    private float _turnRate;
    private float _hangTime;
    private bool _canTrack;
    private bool _canLockOn;

    private CarHealthBehavior carHit;
    private void Start()
    {
        GiveSpeed();
        if (_canLockOn)
        {
            StartCoroutine(hangTimeSequence());
            _carHealthStatus = _target.GetComponent<CarHealthBehavior>();
            _interactableStatus = _target.GetComponent<Interactable>();
        }
    }

    public void SetAdditionalInfo(GameObject giventTarget, float givenTurnRate, float givenHangTime, bool givenlockOn)
    {
        _target = giventTarget;
        _turnRate = givenTurnRate;
        _hangTime = givenHangTime;
        _canLockOn = givenlockOn;
    }

    private void OnTriggerEnter(Collider collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            Explode();
        }
        if(collision.gameObject.GetComponent<CarHealthBehavior>() != null)
        {
            if(collision.gameObject != _immunePlayer)
            {
                carHit = collision.gameObject.GetComponent<CarHealthBehavior>();
                carHit.DamageCar(_projectileDamage, _immunePlayerScript.playerID);
                Explode();
            }
        }
        if(collision.gameObject.GetComponent<Interactable>() != null)
        {
            collision.gameObject.GetComponent<Interactable>().DamageInteractable(_projectileDamage);
            Explode();
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
            if(_target != null)
            {
                _missileTargetRotation = Quaternion.LookRotation(_target.transform.position - transform.position);
                _rigidBody.MoveRotation(Quaternion.RotateTowards(transform.rotation, _missileTargetRotation, _turnRate));
            }
            else
            {
                _canTrack = false;
            }
        }
    }

    private void Update()
    {
        if (_canLockOn)
        {
            if (_carHealthStatus != null && _carHealthStatus.healthCurrent <= 0 && _interactableStatus == null)
            {
                Explode();
            }

            if (_interactableStatus != null && _interactableStatus.interactableHealth <= 0 && _carHealthStatus == null)
            {
                Explode();
            }
        }        
    }

    private void Explode()
    {
        Instantiate(explodeVFX, transform.position, transform.rotation);
        AudioManager.instance.PlayWithoutSpatial("VoidWasp Shot Hit");
        Destroy(gameObject);
    }

}

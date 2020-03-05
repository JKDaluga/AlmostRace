using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lux_TrackingDart : Projectile
{
    
    public GameObject _hitObject;
    public Lux_TrackingLaser laser;

    public float laserHype;
    public float laserDamage;
    public float laserDamageRate;
    public float laserDmgSpeed;
    private bool hitTest;

    void Start()
    {
        hitTest = false;
        GiveSpeed();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<CarHealthBehavior>() != null || other.gameObject.GetComponent<Interactable>()!=null)
        {
            _hitObject = other.gameObject;
            
            Instantiate(laser);
            laser.SetTarget(_hitObject);
        }

    }

    public void setDartInfo(float damage, float damageRate, float hype, float speed)
    {
        laserDamage = damage;
        laserHype = hype;
        laserDamageRate = damageRate;
        laserDmgSpeed = speed;
    }

    void FixedUpdate()
    {
        if (_hitObject != null)
        {
            gameObject.GetComponent<Rigidbody>().velocity = _hitObject.GetComponent<Rigidbody>().velocity;
        }
    }
}

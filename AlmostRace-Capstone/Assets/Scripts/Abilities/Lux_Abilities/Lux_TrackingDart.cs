using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lux_TrackingDart : Projectile
{
    
    public GameObject _hitObject;
    public GameObject laser;

    public float laserHype;
    public float laserDamage;
    public float laserDamageRate;
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
        }

    }

    public void setDartInfo(float damage, float damageRate, float hype)
    {
        laserDamage = damage;
        laserHype = hype;
        laserDamageRate = damageRate;
    }

    void FixedUpdate()
    {
        if (_hitObject != null)
        {
            gameObject.GetComponent<Rigidbody>().velocity = _hitObject.GetComponent<Rigidbody>().velocity;
        }
    }
}

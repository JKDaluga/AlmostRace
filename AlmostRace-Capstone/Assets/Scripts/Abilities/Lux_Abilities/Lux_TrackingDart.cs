using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lux_TrackingDart : Projectile
{
    
    private GameObject _hitObject;
    public GameObject laser;
    private Lux_TrackingLaser _laserScript;
    private Collider objCollider;

    private float _laserDamage;
    private float _laserDamageRate;
    private float _laserDuration;
    private bool hitTest;

    void Start()
    {
        _laserScript = laser.GetComponent<Lux_TrackingLaser>();
        objCollider = gameObject.GetComponent<Collider>();
        hitTest = false;
        GiveSpeed();
    }


    public void setDartInfo(float damage, float damageRate, float laserDuration)
    {
        _laserDamage = damage;
        _laserDamageRate = damageRate;
        _laserDuration = laserDuration;
    }



    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Vehicle") && collision.gameObject != _immunePlayer)
        {
            objCollider.isTrigger = true;
            /* GameObject _spawnedLaser = Instantiate(laser, transform.position, transform.rotation);
             _laserScript = _spawnedLaser.GetComponent<Lux_TrackingLaser>();
             _spawnedLaser.transform.SetParent(collision.gameObject.transform);
             _laserScript.GiveInfo(_laserDamageRate, _laserDamage, _laserDuration, _immunePlayer);
             _laserScript.SetTarget(collision.gameObject);*/
            Lux_TrackingLaser _spawnedLaser = Instantiate(_laserScript, transform.position, transform.rotation);
            _spawnedLaser.transform.SetParent(null);
            _spawnedLaser.GiveInfo(_laserDamageRate, _laserDamage, _laserDuration, _immunePlayer);
            _spawnedLaser.SetTarget(collision.gameObject);
            Destroy(gameObject);
        }

        if (collision.gameObject.CompareTag("Interactable"))
        {
            objCollider.isTrigger = true;
            /* GameObject _spawnedLaser = Instantiate(laser, transform.position, transform.rotation);
             _laserScript = _spawnedLaser.GetComponent<Lux_TrackingLaser>();
             _spawnedLaser.transform.SetParent(collision.gameObject.transform);
             _laserScript.GiveInfo(_laserDamageRate, _laserDamage, _laserDuration, _immunePlayer);
             _laserScript.SetTarget(collision.gameObject);*/
            Lux_TrackingLaser _spawnedLaser = Instantiate(_laserScript, transform.position, transform.rotation);
            _spawnedLaser.transform.SetParent(null);
            _spawnedLaser.GiveInfo(_laserDamageRate, _laserDamage, _laserDuration, _immunePlayer);
            _spawnedLaser.SetTarget(collision.gameObject);
            Destroy(gameObject);

        }

        if (collision.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            _rigidBody.velocity = new Vector3(0, 0, 0);
            _hitObject = collision.gameObject;
        }
    }

}

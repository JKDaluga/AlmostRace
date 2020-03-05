using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lux_TrackingDart : Projectile
{
    
    private GameObject _hitObject;
    public GameObject laser;
    private Lux_TrackingLaser _laserScript;
    private Collider objCollider;
    private bool hasStuckCar;

    private float _laserDamage;
    private float _laserDamageRate;
    private float _laserDuration;
    private bool hitTest;

    void Start()
    {
        hitTest = false;
        GiveSpeed();
        _laserScript = laser.GetComponent<Lux_TrackingLaser>();
        _laserScript.GiveInfo(_laserDamageRate, _laserDamage, _immunePlayer);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<CarHealthBehavior>() != null || other.gameObject.GetComponent<Interactable>()!=null)
        {

        }

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
            transform.SetParent(collision.gameObject.transform);
            hasStuckCar = true;

            laser.SetActive(true);
            _laserScript.SetTarget(collision.gameObject);
            Destroy(gameObject, _laserDuration);
            // StopCoroutine(TrackTargetCar());
            // StartCoroutine(FollowTargetCar());

            _rigidBody.velocity = new Vector3(0, 0, 0);
        }

        if (collision.gameObject.CompareTag("Interactable"))
        {
            objCollider.isTrigger = true;
            _hitObject = collision.gameObject;
            transform.SetParent(collision.gameObject.transform);
            hasStuckCar = true;

            laser.SetActive(true);
            _laserScript.SetTarget(collision.gameObject);
            Destroy(gameObject, _laserDuration);


            //  StopCoroutine(TrackTargetCar());
            // StartCoroutine(FollowTargetCar());
            _rigidBody.velocity = new Vector3(0, 0, 0);
        }

        if (collision.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            _rigidBody.velocity = new Vector3(0, 0, 0);
            _hitObject = collision.gameObject;
        }
    }

}

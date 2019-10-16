using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
 Edouard Borissov

    This script handles the Volt's lightningMissile behavior. It's stats are passed down from the Signature ability script.
    That way, designers only have to set values in one place that is visible on the car object itself.
    Upon exploding, the missile spawns short-lived explosion objects, and persistent lightning cloud objects that disappear after
    a handful of seconds.
     */

    //JASON HELP!

public class Volt_LightningMissile : MonoBehaviour
{
    public GameObject missileExplosion;
    public GameObject lightningCloud;

    private Rigidbody _rigidBody;
    private GameObject _immunePlayer;
    private Vector3 _immunePlayerVelocity;
    private float _speed;
  
    //Missile Variables
    private float _missileExplosionDamage;
    private float _missileSpeed;
    private float _missileFuseLength;
    private float _missileHypeToGain;

    //Cloud Variables
    private float _lightningCloudDuration;

    private float _lightningCloudGrowthRate;

    private float _lightningCloudGrowthAmount;

    private float _lightningCloudMaxSize;

    private float _lightningHypeToGain;


    private void Start()
    {
        _rigidBody = gameObject.GetComponent<Rigidbody>();
        _immunePlayerVelocity = _immunePlayer.gameObject.GetComponent<SphereCarController>().sphere.velocity;
        
        _rigidBody.velocity = transform.TransformDirection(Vector3.forward * _missileSpeed) + _immunePlayerVelocity;
        _speed = _rigidBody.velocity.magnitude;
        Invoke("ExplodeMissile", _missileFuseLength);
    }

    public void FixedUpdate()
    {
        _rigidBody.velocity = transform.forward * _speed;
    }

    public void ExplodeMissile()
    {
        //AudioManager.instance.Play("MissileExplode");
        GameObject spawnedExplosion = Instantiate(missileExplosion, transform.position, transform.rotation);
        GameObject spawnedLightningCloud = Instantiate(lightningCloud, transform.position, transform.rotation);
        
        spawnedExplosion.GetComponent<Volt_ExplosionMissile>().SetImmunePlayer(_immunePlayer);
        spawnedExplosion.GetComponent<Volt_ExplosionMissile>().SetExplosionDamage(_missileExplosionDamage);
        spawnedExplosion.GetComponent<Volt_ExplosionMissile>().SetHypeAmount(_missileHypeToGain);

        spawnedLightningCloud.GetComponent<Volt_LightningCloud>().SetImmunePlayer(_immunePlayer);
        spawnedLightningCloud.GetComponent<Volt_LightningCloud>().SetCloudInfo(_lightningCloudDuration, _lightningCloudGrowthRate, _lightningCloudGrowthAmount, _lightningCloudMaxSize);
        spawnedLightningCloud.GetComponent<Volt_LightningCloud>().SetHypeAmount(_lightningHypeToGain);


        Destroy(gameObject);
        Destroy(spawnedExplosion);
      //  Debug.Log("Missile should have been destroyed!!");
    }

    public void SetMissileInfo(float missileExplosionDamage, float missileFuseLength, float missileSpeed, GameObject immunePlayer, float hypeToGain)
    {
        _immunePlayer = immunePlayer;
        _missileExplosionDamage = missileExplosionDamage;
        _missileFuseLength = missileFuseLength;
        _missileSpeed = missileSpeed;
        _missileHypeToGain = hypeToGain;
       
    }

    public void SetMissileCloudInfo(float cloudDuration, float cloudGrowthRate, float cloudGrowthAmount, float cloudMaxSize, float hypeToGain)
    {
        _lightningCloudDuration = cloudDuration;
        _lightningCloudGrowthRate = cloudGrowthRate;
        _lightningCloudGrowthAmount = cloudGrowthAmount;
        _lightningCloudMaxSize = cloudMaxSize;
        _lightningHypeToGain = hypeToGain;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject != _immunePlayer && other.gameObject.tag != "missileTest")
        {//Checks if the object is neither the immunePlayer nor another missile, as they spawn in the same spot.
            ExplodeMissile();
            Destroy(gameObject);
        }
    }


}

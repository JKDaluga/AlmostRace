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
  
    //Missile Variables
    private float _missileExplosionDamage;
    private float _missileSpeed;
    private float _missileFuseLength;

    //Cloud Variables
    private float _lightningCloudDuration;

    private float _lightningCloudGrowthRate;

    private float _lightningCloudGrowthAmount;

    private float _lightningCloudMaxSize;


    private void Start()
    {
        _rigidBody = gameObject.GetComponent<Rigidbody>();
        _rigidBody.velocity = transform.TransformDirection(Vector3.forward * _missileSpeed );
       // _rigidBody.velocity = transform.TransformDirection(0,0, 1 + _immunePlayer.GetComponent<Rigidbody>().velocity.z) * _missileSpeed;
//JASWEEN HELP! ^^
        Invoke("ExplodeMissile", _missileFuseLength);
    }

    public void ExplodeMissile()
    {
        GameObject spawnedExplosion = Instantiate(missileExplosion, transform.position, transform.rotation);
        GameObject spawnedLightningCloud = Instantiate(lightningCloud, transform.position, transform.rotation);
        spawnedExplosion.GetComponent<Volt_ExplosionMissile>().SetImmunePlayer(_immunePlayer);
        spawnedExplosion.GetComponent<Volt_ExplosionMissile>().SetExplosionDamage(_missileExplosionDamage);

        spawnedLightningCloud.GetComponent<Volt_LightningCloud>().SetImmunePlayer(_immunePlayer);
        spawnedLightningCloud.GetComponent<Volt_LightningCloud>().SetCloudInfo(_lightningCloudDuration, _lightningCloudGrowthRate, _lightningCloudGrowthAmount, _lightningCloudMaxSize);

        Destroy(gameObject);
      //  Debug.Log("Missile should have been destroyed!!");
    }

    public void SetMissileInfo(float missileExplosionDamage, float missileFuseLength, float missileSpeed, GameObject immunePlayer)
    {
        _immunePlayer = immunePlayer;
        _missileExplosionDamage = missileExplosionDamage;
        _missileFuseLength = missileFuseLength;
        _missileSpeed = missileSpeed;
       
    }

    public void SetMissileCloudInfo(float cloudDuration, float cloudGrowthRate, float cloudGrowthAmount, float cloudMaxSize)
    {
        _lightningCloudDuration = cloudDuration;
        _lightningCloudGrowthRate = cloudGrowthRate;
        _lightningCloudGrowthAmount = cloudGrowthAmount;
        _lightningCloudMaxSize = cloudMaxSize;
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

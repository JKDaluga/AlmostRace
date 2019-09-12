using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Volt_LightningMissile : MonoBehaviour
{

    public GameObject missileExplosion;
    public GameObject lightningCloud;

    private Rigidbody _rigidBody;
    private GameObject _immunePlayer;
  

    private float _missileExplosionDamage;
    private float _missileSpeed;
    private float _lightningCloudDuration;
    private float _missileFuseLength;

    private void Start()
    {
        _rigidBody = gameObject.GetComponent<Rigidbody>();
        _rigidBody.velocity = transform.TransformDirection(Vector3.up * _missileSpeed);
        Invoke("ExplodeMissile", _missileFuseLength);
    }

    public void ExplodeMissile()
    {
        GameObject spawnedExplosion = Instantiate(missileExplosion, transform.position, transform.rotation);
        GameObject spawnedLightningCloud = Instantiate(lightningCloud, transform.position, transform.rotation);
        spawnedExplosion.GetComponent<MissileExplosionBehavior>().SetImmunePlayer(_immunePlayer);
        spawnedExplosion.GetComponent<MissileExplosionBehavior>().SetExplosionDamage(_missileExplosionDamage);

        Destroy(gameObject);
    }

    public void SetMissileInfo(float missileExplosionDamage, float missileFuseLength, float missileSpeed, float lightningCloudDuration, GameObject immunePlayer)
    {
        _immunePlayer = immunePlayer;
        _missileExplosionDamage = missileExplosionDamage;
        _missileFuseLength = missileFuseLength;
        _lightningCloudDuration = lightningCloudDuration;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject != _immunePlayer && other.gameObject.GetComponent<Volt_LightningMissile>() == null)
        {//Checks if the object is neither the immunePlayer nor another missile, as they spawn in the same spot.
            ExplodeMissile();
        }
    }


}

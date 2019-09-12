using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Volt_LightningMissile : MonoBehaviour
{
    private Rigidbody _rigidBody;
    private GameObject _immunePlayer;
    public GameObject lightningCloud;
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

    public void SetMissileInfo(float missileExplosionDamage, float missileFuseLength, float missileSpeed, float lightningCloudDuration, GameObject immunePlayer)
    {
        _immunePlayer = immunePlayer;
        _missileExplosionDamage = missileExplosionDamage;
        _missileFuseLength = missileFuseLength;
        _lightningCloudDuration = lightningCloudDuration;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    Creator and developer of script: Eddie Borrisov & Jake Velicer
    Purpose: Abstract Classes for abilities to inherit from 
*/
public abstract class Projectile : MonoBehaviour
{

    public GameObject sparkEffect;
    protected Collider _collider;
    public MeshRenderer meshRenderer;
    public Light pointLight;
    protected GameObject _immunePlayer;
    protected Vector3 _immunePlayerVelocity;
    protected float _speedActual;
    protected VehicleHypeBehavior _immunePlayerScript;
    protected Rigidbody _rigidBody;

    protected float _projectileDamage;
    protected float _projectileSpeed;
    protected float _projectileHype;
    protected bool _isAlive = true;

    // Start is called before the first frame update
    public void Start()
    {
        _collider = gameObject.GetComponent<Collider>();
        _rigidBody = gameObject.GetComponent<Rigidbody>();

        if(meshRenderer == null)
        {
            meshRenderer = gameObject.GetComponent<MeshRenderer>();
        }
     
        _speedActual = _rigidBody.velocity.magnitude;

        Destroy(gameObject, 15.0f);
    }

    protected void GiveSpeed()
    {

        if (_immunePlayer != null && _immunePlayerScript != null)
        {
            _immunePlayerVelocity = _immunePlayer.gameObject.GetComponent<RaycastCar>().GetFlatVelocity();
            _rigidBody.velocity = transform.TransformDirection(Vector3.forward * _projectileSpeed) + _immunePlayerVelocity;
        }
        else
        {
            _rigidBody.velocity = transform.TransformDirection(Vector3.forward * _projectileSpeed);
        }
    }

    public void SetImmunePlayer(GameObject immunePlayer)
    {
        _immunePlayer = immunePlayer;
        if (_immunePlayer.GetComponent<VehicleHypeBehavior>() != null)
        {
            _immunePlayerScript = _immunePlayer.GetComponent<VehicleHypeBehavior>();
        }
    }

    public void SetProjectileInfo(float projectileDamage, float projectileSpeed, float projectileHypeToGain)
    {
        _projectileDamage = projectileDamage;
        _projectileSpeed = projectileSpeed;
        _projectileHype = projectileHypeToGain;
    }

    public GameObject getImmunePlayer()
    {
        return _immunePlayer;
    }

    public float GetProjectileDamage()
    {
        return _projectileDamage;
    }

    public void SetProjectileDamage(float projectileDamage)
    {
        _projectileDamage = projectileDamage;
    }

    public IEnumerator ExplosionEffect()
    {
        _projectileDamage = 0;
        _collider.enabled = false;
        _isAlive = false;
        _rigidBody.velocity = Vector3.zero;
        _rigidBody.useGravity = false;
        _rigidBody.isKinematic = true;
         meshRenderer.enabled = false;
        pointLight.enabled = false;
        if(sparkEffect!= null)
        {
            sparkEffect.SetActive(true);
        }
      if(sparkEffect != null)
        {
            yield return new WaitForSeconds(sparkEffect.GetComponent<ParticleSystem>().main.duration);
        }
      else
        {
            yield return null;
        }
        Destroy(gameObject);
    }
}

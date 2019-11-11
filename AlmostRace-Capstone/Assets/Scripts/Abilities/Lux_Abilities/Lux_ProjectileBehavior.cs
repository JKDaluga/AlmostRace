using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 Eddie B. This code is basically a copy past of Volt_LaserBolt (which will probably get deleted soon).
 I made this script to allow us to edit behavior while keeping a temporary backup.
 This script will no doubt become the final version eventually.
     */

public class Lux_ProjectileBehavior : MonoBehaviour
{

    public GameObject sparkEffect;
    public MeshRenderer meshRenderer;
    public Light pointLight;
    private GameObject _immunePlayer;
    private Vector3 _immunePlayerVelocity;
    private float _speedActual;
    private VehicleHypeBehavior _immunePlayerScript;
    private Rigidbody _rigidBody;

    private float _projectileDamage;
    private float _projectileSpeed;
    private float _projectileHype;
    private bool _isAlive = true;

    // Start is called before the first frame update
    void Start()
    {
        _rigidBody = gameObject.GetComponent<Rigidbody>();
        _immunePlayerVelocity = _immunePlayer.gameObject.GetComponent<SphereCarController>().sphere.velocity;

        _rigidBody.velocity = transform.TransformDirection(Vector3.forward * _projectileSpeed) + _immunePlayerVelocity;
        _speedActual = _rigidBody.velocity.magnitude;

        Destroy(gameObject, 7.0f);
    }

    public void SetImmunePlayer(GameObject immunePlayer)
    {
        _immunePlayer = immunePlayer;
        _immunePlayerScript = _immunePlayer.GetComponent<VehicleHypeBehavior>();
    }

    public void SetProjectileInfo(float projectileDamage, float projectileSpeed, float projectileHypeToGain)
    {
        _projectileDamage = projectileDamage;
        _projectileSpeed = projectileSpeed;
        _projectileHype = projectileHypeToGain;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_isAlive)
        {
            if (other.gameObject != _immunePlayer && other.gameObject.GetComponent<CarHeatManager>() != null)
            {//Checks if the object isn't the immunePlayer and if they are a car.
                other.gameObject.GetComponent<CarHeatManager>().AddHeat(_projectileDamage);
                _immunePlayerScript.AddHype(_projectileHype);
                StartCoroutine(ExplosionEffect());
            }
            else if (other.gameObject != _immunePlayer && other.gameObject.GetComponent<Interactable>() != null)
            {//Checks if the object isn't the immunePlayer and if they are an interactable object.
                other.gameObject.GetComponent<Interactable>().interactingPlayer = _immunePlayer;
                other.gameObject.GetComponent<Interactable>().DamageInteractable(_projectileDamage);
                StartCoroutine(ExplosionEffect());
            }
            else if (other.gameObject != _immunePlayer)
            {
                StartCoroutine(ExplosionEffect());
            }
        }
    }

    private IEnumerator ExplosionEffect()
    {
        _isAlive = false;
        _rigidBody.velocity = Vector3.zero;
        _rigidBody.useGravity = false;
        _rigidBody.isKinematic = true;
        meshRenderer.enabled = false;
        pointLight.enabled = false;
        sparkEffect.SetActive(true);
        yield return new WaitForSeconds(sparkEffect.GetComponent<ParticleSystem>().main.duration);
        Destroy(gameObject);
    }


}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Volt_LaserBolt : MonoBehaviour
{
    public GameObject sparkEffect;
    public MeshRenderer meshRenderer;
    public Light pointLight;
    private GameObject _immunePlayer;
    private Vector3 _immunePlayerVelocity;
    private float _speed;
    private VehicleHypeBehavior _immunePlayerScript;
    private Rigidbody _rigidBody;

    private float _laserDamage;
    private float _laserSpeed;
    private float _laserHype;
    private bool _isAlive = true;
  
    // Start is called before the first frame update
    void Start()
    {
        _rigidBody = gameObject.GetComponent<Rigidbody>();
        _immunePlayerVelocity = _immunePlayer.gameObject.GetComponent<SphereCarController>().sphere.velocity;

        _rigidBody.velocity = transform.TransformDirection(Vector3.forward * _laserSpeed) + _immunePlayerVelocity;
        _speed = _rigidBody.velocity.magnitude;

        Invoke("CleanUp", 7.0f);
    }

    /*public void FixedUpdate()
    {
        _rigidBody.velocity = transform.forward * _speed;
        //_rigidBody.velocity += _immunePlayer.gameObject.GetComponent<SphereCarController>().sphere.velocity;
    }*/

    private void CleanUp()
    {
        Destroy(gameObject);
    }

    public void SetImmunePlayer(GameObject immunePlayer)
    {
        _immunePlayer = immunePlayer;
        _immunePlayerScript = _immunePlayer.GetComponent<VehicleHypeBehavior>();
    }

    public void SetLaserDamage(float laserDamage, float laserSpeed, float laserHype)
    {
        _laserDamage = laserDamage;
        _laserSpeed = laserSpeed;
        _laserHype = laserHype;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_isAlive)
        {
            if (other.gameObject != _immunePlayer && other.gameObject.GetComponent<CarHeatManager>() != null)
            {//Checks if the object isn't the immunePlayer and if they are a car.
                other.gameObject.GetComponent<CarHeatManager>().AddHeat(_laserDamage);
            _immunePlayerScript.AddHype(_laserHype);
                //StartCoroutine(ExplosionEffect());
            }
            else if(other.gameObject != _immunePlayer)
            {
                //StartCoroutine(ExplosionEffect());
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

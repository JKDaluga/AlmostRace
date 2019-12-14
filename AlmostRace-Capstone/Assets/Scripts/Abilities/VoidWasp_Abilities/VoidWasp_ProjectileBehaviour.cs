using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class VoidWasp_ProjectileBehaviour : Projectile
{

    private GameObject _stuckProjectile;

    private float _speedIncrease;

    private float _speedRate;

    private float _speedLimit;

    private float _speedLimitActual;


// //////////EXPLOSION STUFF

    private float _explosionDamage;

    private float _explosionFuse;

    private float _explosionHypeToGain;

    private float _explosionRadius;

    private new void Start()
    {
        base.Start();
        GiveSpeed();
        AudioManager.instance.Play("VoidWasp Shot trail");

        #region DeadCode
        // might be used later
        /*_collider = gameObject.GetComponent<Collider>();
        _rigidBody = gameObject.GetComponent<Rigidbody>();
        if (_immunePlayer != null && _immunePlayerScript != null)
        {
            _immunePlayerVelocity = _immunePlayer.gameObject.GetComponent<SphereCarController>().sphere.velocity;
            _rigidBody.velocity = _immunePlayerVelocity;

            _speedLimitActual = _rigidBody.velocity.magnitude + _speedLimit;
            InvokeRepeating("IncreaseSpeed", 1, _speedRate);
        }
        else
        {
            _rigidBody.velocity = transform.TransformDirection(Vector3.forward * _projectileSpeed);
        }*/
        #endregion
    }

    public void IncreaseSpeed()
    {
        _rigidBody.velocity += transform.TransformDirection(0,0, _speedIncrease);
        if (_rigidBody.velocity.z >= _speedLimitActual)
        {
            CancelInvoke();
        }
    }

    public void GiveInfo(float speedIncrease, float speedRate, float speedLimit, GameObject stuckProjectile, float explosionDamage, float explosionFuse, float explosionHypeToGain, float explosionRadius)
    {
        _speedIncrease = speedIncrease;
        _speedRate = speedRate;
        _speedLimit = speedLimit;
        _explosionDamage = explosionDamage;
        _explosionFuse = explosionFuse;
        _explosionHypeToGain = explosionHypeToGain;
        _stuckProjectile = stuckProjectile;
        _explosionRadius = explosionRadius;
        //Debug.Log("Explosion Fuse at Projectile is: " + _explosionFuse);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject != _immunePlayer && collision.gameObject.GetComponent<CarHeatManager>() != null)
        {//Checks if the object isn't the immunePlayer and if they are a car.
            Debug.Log("1 Projectile Hit: " + collision.gameObject.name);
            collision.gameObject.GetComponent<CarHeatManager>().AddHeat(_projectileDamage);

            collision.gameObject.GetComponent<CinemachineImpulseSource>().m_ImpulseDefinition.m_AmplitudeGain = .25f;
            collision.gameObject.GetComponent<CinemachineImpulseSource>().m_ImpulseDefinition.m_FrequencyGain = .25f;

            collision.gameObject.GetComponent<CinemachineImpulseSource>().GenerateImpulse();
            _immunePlayerScript.AddHype(_projectileHype, "Damage:");

            AudioManager.instance.Play("VoidWasp Shot hit");
        }
        else if (collision.gameObject != _immunePlayer && collision.gameObject.GetComponent<Interactable>() != null)
        { //Hits Interactable
            collision.gameObject.GetComponent<Interactable>().interactingPlayer = _immunePlayer;
            collision.gameObject.GetComponent<Interactable>().DamageInteractable(_projectileDamage);
            Debug.Log("2 Projectile Hit: " + collision.gameObject.name);

            AudioManager.instance.Play("VoidWasp Shot hit");
        }

      
            ContactPoint contact = collision.contacts[0];
            Quaternion rot = Quaternion.FromToRotation(Vector3.up, contact.normal);
            Vector3 pos = contact.point;

     
        
            // spawn game object into collided position
            GameObject spawnedClone2 = Instantiate(_stuckProjectile, pos, rot);
        if ((collision.gameObject != _immunePlayer))
        {
            spawnedClone2.transform.SetParent(collision.gameObject.transform);
            spawnedClone2.GetComponent<VoidWasp_Projectile_Explosion>().LightFuse();
        }

            //TODO
            spawnedClone2.GetComponent<VoidWasp_Projectile_Explosion>().GiveInfo(_explosionDamage, _explosionFuse, _explosionHypeToGain, _explosionRadius, _immunePlayer);
          

        //TODO Replace this with particle explosions or w/e u want
        Destroy(gameObject);
    }
}

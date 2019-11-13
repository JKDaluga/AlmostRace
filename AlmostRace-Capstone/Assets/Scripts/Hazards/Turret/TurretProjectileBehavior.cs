using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 
    Simple code for the Turret projectiles. They are currently behaving like lasers, can be changed to anything.
     
     */

public class TurretProjectileBehavior : MonoBehaviour
{

    private Rigidbody _rigidBody;
  
    private float _projectileDamage;
    public float _projectileSpeed;
    private Collider _collider;
    [Header("Particle Variables...................................................................")]
    public GameObject sparkEffect;
    public MeshRenderer meshRenderer;
    public Light pointLight;
    private GameObject _immunePlayer;
    // Start is called before the first frame update
    void Start()
    {
        _collider = gameObject.GetComponent<Collider>();
        _rigidBody = gameObject.GetComponent<Rigidbody>();
        _rigidBody.velocity = transform.TransformDirection(Vector3.up * _projectileSpeed);    
        Destroy(gameObject, 7.0f);
    }

    public void SetProjectileInfo(float projectileDamage, float projectileSpeed, GameObject immunePlayer)
    {
        _projectileDamage = projectileDamage;
        _projectileSpeed = projectileSpeed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(_immunePlayer != other.gameObject)
        {
            if (other.gameObject.GetComponent<CarHeatManager>() != null)
            {//if other is a car
                other.gameObject.GetComponent<CarHeatManager>().AddHeat(_projectileDamage);
                Debug.Log("Damage done to player: " + _projectileDamage);
                StartCoroutine(ExplosionEffect());
            }
            else if (other.gameObject.GetComponent<Interactable>() != null)
            {//Checks if the object isn't the immunePlayer and if they are an interactable object.

                other.gameObject.GetComponent<Interactable>().DamageInteractable(_projectileDamage);
                StartCoroutine(ExplosionEffect());
            }
            else
            {
                StartCoroutine(ExplosionEffect());
            }
        }
       
    }

    public IEnumerator ExplosionEffect()
    {
        _collider.enabled = false;
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

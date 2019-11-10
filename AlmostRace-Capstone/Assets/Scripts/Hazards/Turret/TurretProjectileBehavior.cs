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
    // Start is called before the first frame update
    void Start()
    {
        _rigidBody = gameObject.GetComponent<Rigidbody>();
        _rigidBody.velocity = transform.TransformDirection(Vector3.up * _projectileSpeed);    
        Destroy(gameObject, 7.0f);
    }



    public void SetProjectileInfo(float projectileDamage, float projectileSpeed)
    {
        _projectileDamage = projectileDamage;
        _projectileSpeed = projectileSpeed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.GetComponent<CarHeatManager>() != null)
        {//if other is a car
            other.gameObject.GetComponent<CarHeatManager>().AddHeat(_projectileDamage);
            Debug.Log("Damage done to player: " + _projectileDamage);
            Destroy(gameObject);
        }     
    }
}

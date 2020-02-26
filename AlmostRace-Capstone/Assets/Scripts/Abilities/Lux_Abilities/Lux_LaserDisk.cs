using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*Eddie Borissov
 Code that handles the functionality of the laser disks fired by the 
 Lux's offensive ability */

public class Lux_LaserDisk : Projectile
{
    private float _laserHype;

    public GameObject explodeVFX;

    int ignore;

    public Transform laserEmitterLeft;
    public Transform laserEmitterRight;

    private float _laserDamage;
    private float _laserPulseRate;

    private GameObject _rightLaserHit;
    private GameObject _leftLaserHit;

    private RaycastHit rayHit;

    private void Start()
    {
        ignore = ~LayerMask.GetMask("AISight", "Detector", "Ignore Abilities");

        GiveSpeed();
        InvokeRepeating("DealDamage", 0, _laserPulseRate);
    }

    private void Update()
    {
        PulseLasers();
    }

    /// <summary>
    /// This function is called frequently, many times a second, in order to spawn the laser code and damage anything it hits.
    /// This is done in lue of DoT lists, such as the ones used with DoT colliders. Raycasts don't have an OnTriggerExit, after all.
    /// </summary>
    public void PulseLasers()
    {
        //Left laser code
        if (Physics.Raycast(laserEmitterLeft.position, laserEmitterLeft.TransformDirection(Vector3.forward), out rayHit, Mathf.Infinity, ignore))
        {
            //if it hit a car
            if (rayHit.collider.gameObject.GetComponent<CarHealthBehavior>() != null)
            {//hit a car
                if (rayHit.collider.gameObject != _immunePlayer)
                {
                    Debug.Log("Car found by laserEmitterLeft");
                    _leftLaserHit = rayHit.collider.gameObject;
                    // rayHit.collider.gameObject.GetComponent<CarHealthBehavior>().DamageCar(_laserDamage);
                    //_immunePlayer.GetComponent<VehicleHypeBehavior>().AddHype(_laserHype, "Zapped");
                }

            }

            //if it hit an interactable
            if (rayHit.collider.gameObject.GetComponent<Interactable>() != null)
            {//hit an interactable
                Debug.Log("Interactable found by laserEmitterLeft");
                _leftLaserHit = rayHit.collider.gameObject;
                //rayHit.collider.gameObject.GetComponent<Interactable>().DamageInteractable(_laserDamage);
                //_immunePlayer.GetComponent<VehicleHypeBehavior>().AddHype(_laserHype, "Zapped");
            }

            //Debug.DrawRay(laserEmitterLeft.position, laserEmitterLeft.TransformDirection(Vector3.forward) * rayHit.distance, Color.red);
        }

        //Right laser code
        if (Physics.Raycast(laserEmitterRight.position, laserEmitterRight.TransformDirection(Vector3.forward), out rayHit, Mathf.Infinity, ignore))
        {
            //if it hit a car
            if (rayHit.collider.gameObject.GetComponent<CarHealthBehavior>() != null)
            {//hit a car
                if (rayHit.collider.gameObject != _immunePlayer)
                {
                    //Debug.Log("Car found by laserEmitterRight");
                    _rightLaserHit = rayHit.collider.gameObject;
                    //rayHit.collider.gameObject.GetComponent<CarHealthBehavior>().DamageCar(_laserDamage);
                    //_immunePlayer.GetComponent<VehicleHypeBehavior>().AddHype(_laserHype, "Zapped");
                }
            }

            //if it hit an interactable
            if (rayHit.collider.gameObject.GetComponent<Interactable>() != null)
            {//hit an interactable
                //Debug.Log("Interactable found by laserEmitterRight");
                _rightLaserHit = rayHit.collider.gameObject;
                //rayHit.collider.gameObject.GetComponent<Interactable>().DamageInteractable(_laserDamage);
                //_immunePlayer.GetComponent<VehicleHypeBehavior>().AddHype(_laserHype, "Zapped");
            }

            // Debug.DrawRay(laserEmitterRight.position, laserEmitterRight.TransformDirection(Vector3.forward) * rayHit.distance, Color.red);

        }

    }

    /// <summary>
    /// might seem a bit extra just for one variable, but it makes more sense than making it 
    /// public and potentially having a designer accidentally fill it out here, or worse yet adding it to Projectile,
    /// which would make 0 sense, since most projectiles only have 1 hype variable (currently).
    /// </summary>
    /// <param name="diskHype"> The amount of hype you want to gain from scoring a direct hit </param>
    public void SetDiskInfo(float laserDamage, float laserDamageRate, float laserHype)
    {
        _laserDamage = laserDamage;
        _laserHype = laserHype;
        _laserPulseRate = laserDamageRate;
    }

    public void DealDamage()
    {
        if (_leftLaserHit != null)
        {
            if(_leftLaserHit.GetComponent<CarHealthBehavior>() != null)
            {
                _leftLaserHit.GetComponent<CarHealthBehavior>().DamageCar(_laserDamage);

                if (_leftLaserHit.GetComponent<CarHealthBehavior>().isDead)
                {
                    _leftLaserHit = null;
                }
            }

            if (_leftLaserHit != null && _leftLaserHit.GetComponent<Interactable>() != null)
            {
                _leftLaserHit.GetComponent<Interactable>().DamageInteractable(_laserDamage);

                if (_leftLaserHit.GetComponent<Interactable>().interactableHealth <= 0)
                {
                    _leftLaserHit = null;
                }
            }
        }

        if (_rightLaserHit != null)
        {
            if (_rightLaserHit.GetComponent<CarHealthBehavior>() != null)
            {
                _rightLaserHit.GetComponent<CarHealthBehavior>().DamageCar(_laserDamage);

                if (_rightLaserHit.GetComponent<CarHealthBehavior>().isDead)
                {
                    _rightLaserHit = null;
                }
            }

            if (_rightLaserHit != null && _rightLaserHit.GetComponent<Interactable>() != null)
            {
                _rightLaserHit.GetComponent<Interactable>().DamageInteractable(_laserDamage);

                if (_rightLaserHit.GetComponent<Interactable>().interactableHealth <= 0)
                {
                    _rightLaserHit = null;
                }
            }
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            Instantiate(explodeVFX, transform.position, transform.rotation);
            Destroy(gameObject);
        }

        if (collision.gameObject.GetComponent<CarHealthBehavior>() != null)
        {
            if (collision.gameObject != _immunePlayer)
            {
                Instantiate(explodeVFX, transform.position, transform.rotation);
                collision.gameObject.GetComponent<CarHealthBehavior>().DamageCar(_projectileDamage);
                _immunePlayer.GetComponent<VehicleHypeBehavior>().AddHype(_projectileHype, "Deadshot");
                Destroy(gameObject);
            }

        }

        if (collision.gameObject.GetComponent<Interactable>() != null)
        {
            Instantiate(explodeVFX, transform.position, transform.rotation);
            collision.gameObject.GetComponent<Interactable>().DamageInteractable(_projectileDamage);
            _immunePlayer.GetComponent<VehicleHypeBehavior>().AddHype(_projectileHype / 2, "Vandal!!");
            Destroy(gameObject);
        }


    }


}

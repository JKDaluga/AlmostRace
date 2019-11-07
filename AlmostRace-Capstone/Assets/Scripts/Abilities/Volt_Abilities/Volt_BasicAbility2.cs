using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 Author: Eddie Borissov
 Purpose: Handles the Volt's long range quad laser cannons.
*/

/*
 * Edited by Robyn Riley 11/5/19
 * Added functionality to make missiles fire in direction camera is facing
 */

public class Volt_BasicAbility2 : HeatAbility
{

    public GameObject laserBolt;
    private Volt_LaserBolt _voltLaserBoltInfo;

    [Header("Ability Values")]

    private bool _isFiring = false;
    private bool _canFireLaser = true;

    [Tooltip("How much Hype is given per laser bolt.")]
    public float laserHypeToGain;

    [Tooltip("How much damage is done per laser bolt.")]
    public float laserDamage;

    [Tooltip("How quickly each laser moves.")]
    public float laserSpeed;

    [Tooltip("How quickly the laser are fired, smaller number means more lasers!")]
    public float laserRateOfFire;
    private float _cooldownTime = 0;

    [Tooltip("How fast firing the quad cannon builds heat.")]
    public float selfDamageRate;

    public List<Transform> laserMuzzles;
    private int _currentMuzzle;
    private int _muzzleIterator = 1;



    private void Start()
    {
        _currentMuzzle = 1;
        carHeatInfo = gameObject.GetComponent<CarHeatManager>();
    }

    private IEnumerator LaserCooldown()
    {
        float tempTime = laserRateOfFire;

        while (tempTime > 0)
        {
            tempTime -= Time.deltaTime;
            yield return null;
        }
        _canFireLaser = true;
    }

    public override void ActivateAbility()
    {
        if (!_isFiring)
        {
            _isFiring = true;
            InvokeRepeating("AddHeat", 0, selfDamageRate);
            InvokeRepeating("FireLaser", 0, laserRateOfFire);
        }

    }

    public override void DeactivateAbility()
    {
        if (_isFiring)
        {
            _isFiring = false;
            CancelInvoke("AddHeat");
            CancelInvoke("FireLaser");     
        }
    }

    public void FireLaser()
    {
        if(_canFireLaser)
        {
            AudioManager.instance.Play("Blaster1");
            foreach(Transform i in laserMuzzles)
            {
                i.LookAt(GetComponent<SphereCarController>().aimPos.transform);
            }

            GameObject laser = Instantiate(laserBolt, laserMuzzles[_currentMuzzle].position, laserMuzzles[_currentMuzzle].rotation);
            _voltLaserBoltInfo = laser.GetComponent<Volt_LaserBolt>();
            _voltLaserBoltInfo.SetImmunePlayer(gameObject);
            _voltLaserBoltInfo.SetLaserDamage(laserDamage, laserSpeed, laserHypeToGain);
            _canFireLaser = false;
            StartCoroutine(LaserCooldown());
            if (_currentMuzzle == (laserMuzzles.Count - 1) || _currentMuzzle == 0)
            {
                _muzzleIterator *= -1;
            }
            _currentMuzzle += _muzzleIterator;
        }
    }


    protected override void AddHeat()
    {
        carHeatInfo.AddHeat(selfHeatDamage);           
    }
}

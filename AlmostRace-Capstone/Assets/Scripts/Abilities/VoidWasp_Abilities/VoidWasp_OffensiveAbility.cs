using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Mike Romeo: Created document. This script will handle the void wasps offensive abillity
 */

public class VoidWasp_OffensiveAbility : HeatAbility
{
    public GameObject voidwaspProjectile;
    private Lux_ProjectileBehavior _voidwaspProjectileScript;

    [Header("Ability Values")]

    private bool _isFiring = false;
    private bool _canFire = true;

    [Tooltip("How much Hype is given per projectile.")]
    public float projectileHypeToGain;

    [Tooltip("How much damage is done per projectile bolt.")]
    public float projectileDamage;

    [Tooltip("How quickly each projectile moves.")]
    public float projectileSpeed;


    /*
     * Todo: How many bullets it shoot
     * 
     * Todo: Bullet spread
     */


    /* use this to set delay between shots*/
    [Tooltip("How quickly the projectile are fired, smaller number means more projectiles!")]
    public float rateOfFire;
    private float _cooldownTime = 0;

    [Tooltip("How fast firing projectiles builds heat.")]
    public float selfDamageRate;

    public Transform waspAimDirection;
    private Transform _aimPosActual;

    // ask eddie what this is for
    public Transform voidMuzzle;

    // Start is called before the first frame update
    void Start()
    {
        //_currentMuzzle = 1;
        carHeatInfo = gameObject.GetComponent<CarHeatManager>();
        _aimPosActual = GetComponent<SphereCarController>().aimPos.transform;
        StartCoroutine(TurretAim());
    }

    private IEnumerator TurretAim()
    {
        while (true)
        {
            waspAimDirection.LookAt(_aimPosActual);
            yield return null;
        }
    }

    private IEnumerator AbilityRateOfFire()
    {
        float tempTime = rateOfFire;

        while (tempTime > 0)
        {
            tempTime -= Time.deltaTime;
            yield return null;
        }
        _canFire = true;
    }


    public void FireProjectile()
    {
        if (_canFire)
        {

            // Todo: Instantia multiple projectiles at once. Each with slightly different rotation
            // This rotation is based on the spread

            GameObject projectile = Instantiate(voidwaspProjectile, voidMuzzle.position, voidMuzzle.rotation);
            
            // todo: When hit, creates explosion particle effect. 

            _voidwaspProjectileScript = projectile.GetComponent<Lux_ProjectileBehavior>();
            _voidwaspProjectileScript.SetImmunePlayer(gameObject);
            _voidwaspProjectileScript.SetProjectileInfo(projectileDamage, projectileSpeed, projectileHypeToGain);
            _canFire = false;

            //StartCoroutine(AbilityRateOfFire());
            /*if (_currentMuzzle == (muzzles.Count - 1) || _currentMuzzle == 0)
            {
                _muzzleIterator *= -1;
            }
            _currentMuzzle += _muzzleIterator;*/
        }
    }

    public override void ActivateAbility()
    {
        if (!_isFiring)
        {
            _isFiring = true;
            InvokeRepeating("AddHeat", 0, selfDamageRate);
            InvokeRepeating("FireProjectile", 0, rateOfFire);
        }
    }

    public override void DeactivateAbility()
    {
        if (_isFiring)
        {
            _isFiring = false;
            CancelInvoke("AddHeat");
            CancelInvoke("FireProjectile");
        }
    }

    protected override void AddHeat()
    {
        carHeatInfo.AddHeat(selfHeatDamage);
    }

}

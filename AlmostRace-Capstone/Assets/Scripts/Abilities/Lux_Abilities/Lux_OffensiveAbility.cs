using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Eddie B. The code that handles the Lux's basic ability.
 * Very similar to Volt_BasicAbility 2, but refactored and modified to perform better and fit with camera aiming
 */

public class Lux_OffensiveAbility : HeatAbility
{
    public GameObject luxProjectile;
    private Lux_ProjectileBehavior _luxProjectileScript;

    [Header("Ability Values")]

    private bool _isFiring = false;
    private bool _canFire = true;

    [Tooltip("How much Hype is given per projectile.")]
    public float projectileHypeToGain;

    [Tooltip("How much damage is done per projectile bolt.")]
    public float projectileDamage;

    [Tooltip("How quickly each projectile moves.")]
    public float projectileSpeed;

    [Tooltip("How quickly the projectile are fired, smaller number means more projectiles!")]
    public float rateOfFire;
    private float _cooldownTime = 0;

    [Tooltip("How fast firing projectiles builds heat.")]
    public float selfDamageRate;

    public Transform rotaryTurret;
    private Transform _aimPosActual;

    public List<Transform> muzzles;
    private int _currentMuzzle;
    private int _muzzleIterator = 1;



    // Start is called before the first frame update
    void Start()
    {
        _currentMuzzle = 1;
        carHeatInfo = gameObject.GetComponent<CarHeatManager>();
        _aimPosActual = GetComponent<SphereCarController>().aimPos.transform;
        StartCoroutine(TurretAim());

    }

    private IEnumerator TurretAim()
    {
        while(true)
        {
            rotaryTurret.LookAt(_aimPosActual);
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
           // AudioManager.instance.Play("Blaster1");
            foreach (Transform i in muzzles)
            {
                i.LookAt(_aimPosActual);
            }

            GameObject projectile = Instantiate(luxProjectile, muzzles[_currentMuzzle].position, muzzles[_currentMuzzle].rotation);

            _luxProjectileScript = projectile.GetComponent<Lux_ProjectileBehavior>();
            _luxProjectileScript.SetImmunePlayer(gameObject);
            _luxProjectileScript.SetProjectileInfo(projectileDamage, projectileSpeed, projectileHypeToGain);
            _canFire = false;
            StartCoroutine(AbilityRateOfFire());
            if (_currentMuzzle == (muzzles.Count - 1) || _currentMuzzle == 0)
            {
                _muzzleIterator *= -1;
            }
            _currentMuzzle += _muzzleIterator;
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

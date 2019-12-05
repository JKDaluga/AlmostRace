using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Mike Romeo: Created document. This script will handle the void wasps offensive abillity
 */

public class VoidWasp_OffensiveAbility : HeatAbility
{
    public GameObject voidwaspProjectile;
    private VoidWasp_ProjectileBehaviour _voidwaspProjectileScript;

    [Header("Ability Values")]

    private bool _isFiring = false;
    private bool _canFire = true;

    [Tooltip("How much Hype is given per projectile.")]
    public float projectileHypeToGain;

    [Tooltip("How much damage is done per projectile bolt.")]
    public float projectileDamage;

    public float speedIncrease;

    public float speedRate;

    public float speedLimit;

    [Tooltip("How quickly each projectile moves.")]
    public float projectileSpeed;

    [Tooltip("How many projectile it shoots in bursts")]
    public int projectileCount = 8;

    [Tooltip("How much the bullets should spread from each other")]
    public float shotSpread;

    private List<Quaternion> _projectiles;

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

    private void Awake()
    {
        _projectiles = new List<Quaternion>(projectileCount);
        for (int i = 0; i < projectileCount; i++)
        {
            _projectiles.Add(Quaternion.Euler(Vector3.zero));
        }
    }

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

            // todo: When hit, creates explosion particle effect. 
            for (int i = 0; i < projectileCount; i++)
            {
                _projectiles[i] = Random.rotation;

                GameObject projectile = Instantiate(voidwaspProjectile, voidMuzzle.position, voidMuzzle.rotation);
                _voidwaspProjectileScript = projectile.GetComponent<VoidWasp_ProjectileBehaviour>();
                _voidwaspProjectileScript.SetImmunePlayer(gameObject);
                _voidwaspProjectileScript.SetProjectileInfo(projectileDamage, projectileSpeed, projectileHypeToGain);
                _voidwaspProjectileScript.GiveInfo(speedIncrease, speedRate, speedLimit);
                projectile.transform.Rotate(Random.Range(-shotSpread/2, shotSpread/2), Random.Range(-shotSpread, shotSpread), 0);

                //projectile.transform.rotation = Quaternion.RotateTowards(projectile.transform.rotation, _projectiles[i], shotSpread);

            }
           
            _canFire = false;

            StartCoroutine(AbilityRateOfFire());
            //cameraAnimator.SetTrigger("shotFired");
            // Probably use this for boost but changing fov while shooting does not really work.
            /*ChangeFov(cameraChangeDuration, cameraFovTarget);
            Invoke("ResetFov", cameraChangeDuration);*/
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

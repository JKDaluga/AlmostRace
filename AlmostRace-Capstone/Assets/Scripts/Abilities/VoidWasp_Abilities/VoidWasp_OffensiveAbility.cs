using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 *  Mike R.
 *  Eddie B.
 *  Robin R.
 *
 *  VoidWasp_OffensiveAbility spawns projectiles and shoots them in a predicted path.
 *  The spawned projectile spawns in a random rotation spread.
 **/

public class VoidWasp_OffensiveAbility : HeatAbility
{

    [Header("Ability Values")]
    [Space(30)]
    public GameObject voidwaspProjectile;
    private VoidWasp_ProjectileBehaviour _voidwaspProjectileScript;

    private bool _isFiring = false;
    private bool _canFire = true;

    /* use this to set delay between shots*/
    [Tooltip("How quickly the projectile are fired, smaller number means more projectiles!")]
    public float rateOfFire;
    private float _cooldownTime = 0;

    [Tooltip("How fast firing projectiles builds heat.")]
    public float selfDamageRate;

    public Transform waspAimDirection;
    private Transform _aimPosActual;


    [Header("Projectile Variables")]
    [Space(30)]

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

    private List<Quaternion> _projectileRotations;

    [Header("Explosion Variables")]
    [Space(30)]

    public GameObject stuckProjectile;

    public float explosionDamage = 2f;

    public float explosionFuse = 1f;

    public float explosionRadius = 1f;

    public float explosionHypeToGain = 5f;

    public Transform voidMuzzle;

    private void Awake()
    {
        _projectileRotations = new List<Quaternion>(projectileCount);
        for (int i = 0; i < projectileCount; i++)
        {
            _projectileRotations.Add(Quaternion.Euler(Vector3.zero));
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        carHeatInfo = gameObject.GetComponent<CarHeatManager>();
        _aimPosActual = GetComponent<SphereCarController>().aimPos.transform;
        StartCoroutine(TurretAim());

    }

    private IEnumerator TurretAim()
    {
        while (true)
        {
            if (GetComponent<AimAssistant>().target != null && GetComponent<AimAssistant>().target.GetComponent<SphereCarController>() != null)
            {
                Vector3 assistedPos = PredictedPosition(_aimPosActual.position, transform.position, GetComponent<AimAssistant>().target.GetComponent<Rigidbody>().velocity, projectileSpeed);
                waspAimDirection.LookAt(assistedPos);
            }
            else
            {
                waspAimDirection.LookAt(_aimPosActual);
            }
            yield return null;
        }
    }

    private Vector3 PredictedPosition(Vector3 targetPosition, Vector3 shooterPosition, Vector3 targetVelocity, float projectileSpeed)
    {
        Vector3 displacement = targetPosition - shooterPosition;
        float targetMoveAngle = Vector3.Angle(-displacement, targetVelocity) * Mathf.Deg2Rad;
        //if the target is stopping or if it is impossible for the projectile to catch up with the target (Sine Formula)
        if (targetVelocity.magnitude == 0 || targetVelocity.magnitude > projectileSpeed && Mathf.Sin(targetMoveAngle) / projectileSpeed > Mathf.Cos(targetMoveAngle) / targetVelocity.magnitude)
        {
            //  Debug.Log("Position prediction is not feasible.");
            return targetPosition;
        }
        //also Sine Formula
        float shootAngle = Mathf.Asin(Mathf.Sin(targetMoveAngle) * targetVelocity.magnitude / projectileSpeed);
        return targetPosition + targetVelocity * displacement.magnitude / Mathf.Sin(Mathf.PI - targetMoveAngle - shootAngle) * Mathf.Sin(shootAngle) / targetVelocity.magnitude;
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
            for (int i = 0; i < projectileCount; i++)
            {
                _projectileRotations[i] = Random.rotation;

                GameObject projectile = Instantiate(voidwaspProjectile, voidMuzzle.position, voidMuzzle.rotation);
                _voidwaspProjectileScript = projectile.GetComponent<VoidWasp_ProjectileBehaviour>();
                _voidwaspProjectileScript.SetImmunePlayer(gameObject);
                _voidwaspProjectileScript.SetProjectileInfo(projectileDamage, projectileSpeed, projectileHypeToGain);

                _voidwaspProjectileScript.GiveInfo(speedIncrease, speedRate, speedLimit, stuckProjectile, explosionDamage, explosionFuse, explosionHypeToGain, explosionRadius);

                projectile.transform.Rotate(0, Random.Range(-shotSpread, shotSpread), 0);
                //projectile.transform.rotation = Quaternion.RotateTowards(projectile.transform.rotation, _projectiles[i], shotSpread);
                AudioManager.instance.Play("VoidWasp Shot");
            }

            _canFire = false;
            StartCoroutine(AbilityRateOfFire());
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
        carHeatInfo.DamageCar(selfHeatDamage);
    }



}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lux_Attack2 : Ability
{
    public GameObject energyBlade;
    private Lux_EnergyBlade _energyBladeScript;

    public bool shouldAnimate = true;

    public ParticleSystem laserBlastLeft;
    public ParticleSystem laserBlastRight;
    public ParticleSystem laserSmokeLeft;
    public ParticleSystem laserSmokeRight;

    public Animator leftGunAnimator;
    public Animator rightGunAnimator;
    public Animator leftCap;
    public Animator rightCap;

    public Transform muzzle;
    
    public int bladesToSpawn = 3;

    private int bladesToSpawnActual; 

    public float bladeDamage = 50;

    public float bladeSpeed = 300;

    public float bladeOffset = 0.75f;

    public float bladeDuration = 10;

    public float growthRate = .1f;

    public float growthAmount = .001f;

    public float growthLimit = .03f;

    public void Start()
    {
        bladesToSpawnActual = bladesToSpawn;
    }

    public override void ActivateAbility()
    {
        InvokeRepeating("SpawnBlade", 0, bladeOffset);
    }


    public void SpawnBlade()
    {
        if(bladesToSpawn > 0)
        {
            bladesToSpawn--;

            GameObject spawnedBlade = Instantiate(energyBlade, muzzle.position, muzzle.rotation);
            laserBlastLeft.Play();
            laserBlastRight.Play();
            _energyBladeScript = spawnedBlade.GetComponent<Lux_EnergyBlade>();

            _energyBladeScript.SetProjectileInfo(bladeDamage, bladeSpeed, 0);

            _energyBladeScript.GiveInfo(growthRate, growthAmount, growthLimit);

            _energyBladeScript.SetImmunePlayer(gameObject);

            AudioManager.instance.Play("Lux Shooting", transform);
        }
        else
        {
            bladesToSpawn = bladesToSpawnActual;
            CancelInvoke("SpawnBlade");
        }

    }

    public override void DeactivateAbility()
    {
        bladesToSpawn = bladesToSpawnActual;
    }



    public override void AbilityOnCooldown()
    {
        if (shouldAnimate)
        {
            Debug.Log("AbilityOnCooldown");
        }

    }

    public override void AbilityOffOfCooldown()
    {
        if(shouldAnimate)
        {
            //Debug.Log("AbilityOffOfCooldown");
            leftGunAnimator.SetTrigger("LeftGunDown");
            rightGunAnimator.SetTrigger("RightGunDown");
            leftCap.SetTrigger("LeftCapClose");
            rightCap.SetTrigger("RightCapClose");
            laserSmokeLeft.gameObject.SetActive(false);
            laserSmokeRight.gameObject.SetActive(false);
        }

    }

    public override void AbilityInUse()
    {
        if (shouldAnimate)
        {
            //Debug.Log("AbilityInUse");
            leftGunAnimator.SetTrigger("LeftGunUp");
            rightGunAnimator.SetTrigger("RightGunUp");
            leftCap.SetTrigger("LeftCapOpen");
            rightCap.SetTrigger("RightCapOpen");
            laserSmokeLeft.gameObject.SetActive(true);
            laserSmokeRight.gameObject.SetActive(true);
        }

    }
}

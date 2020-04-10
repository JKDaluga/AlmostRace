using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lux_Attack2 : Ability
{
    public GameObject energyBlade;
    private Lux_EnergyBlade _energyBladeScript;

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

            _energyBladeScript = spawnedBlade.GetComponent<Lux_EnergyBlade>();

            _energyBladeScript.SetProjectileInfo(bladeDamage, bladeSpeed, 0);

            _energyBladeScript.GiveInfo(growthRate, growthAmount, growthLimit);

            _energyBladeScript.SetImmunePlayer(gameObject);
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

    }

    public override void AbilityOffOfCooldown()
    {

    }

    public override void AbilityInUse()
    {

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Volt_SignatureAbility : Ability
{
    /*
     * Edouard Borissov
     * This script handles the Volt's lightning missile ability. Included below are all of the values that will be
     * passed down to the missiles, their explosions, and the lightning clouds they spawn. All variables are visible here so
     * that designers don't have to search across multiple objects and prefabs to set values.
     */
    [Header("Missile Info")]

    [Tooltip("Put missile here.")]
    public GameObject lightningMissile;

    [Tooltip("The angle amount that each side missile is rotated when fired.")]
    public float sideMissileOffset;

    [Tooltip("How fast the missile moves.")]
    public float missileSpeed;

    [Tooltip("Heat damage done from missile explosion.")]
    public float missileExplosionDamage;

    [Tooltip("Time before missile explodes.")]
    public float missileFuseLength;

    [Tooltip("Where the missiles spawn from")]
    public Transform missileSpawnLocation;

    [Header("Lightning Cloud Info")]

    [Tooltip("How long lightning clouds persist in scene.")]
    public float lightningCloudDuration;

    [Tooltip("How often the cloud grows. Smaller number = more often.")]
    public float lightningCloudGrowthRate;

    [Tooltip("How much the cloud grows per tic.")]
    public float lightningCloudGrowthAmount;

    [Tooltip("How large the cloud can get.")]
    public float lightningCloudMaxSize;




    public override void ActivateAbility()
    {
        GameObject spawnedMissile1 = Instantiate(lightningMissile, missileSpawnLocation.transform.position, missileSpawnLocation.transform.rotation);
        GameObject spawnedMissile2 = Instantiate(lightningMissile, missileSpawnLocation.transform.position, missileSpawnLocation.transform.rotation);
        GameObject spawnedMissile3 = Instantiate(lightningMissile, missileSpawnLocation.transform.position, missileSpawnLocation.transform.rotation);

        //give each missile its Missile Info
        spawnedMissile1.GetComponent<Volt_LightningMissile>().SetMissileInfo(missileExplosionDamage, missileFuseLength, missileSpeed, gameObject);
        spawnedMissile2.GetComponent<Volt_LightningMissile>().SetMissileInfo(missileExplosionDamage, missileFuseLength, missileSpeed, gameObject);
        spawnedMissile3.GetComponent<Volt_LightningMissile>().SetMissileInfo(missileExplosionDamage, missileFuseLength, missileSpeed, gameObject);

        //give each missile its Lightning Cloud info
        spawnedMissile1.GetComponent<Volt_LightningMissile>().SetMissileCloudInfo(lightningCloudDuration, lightningCloudGrowthRate, lightningCloudGrowthAmount, lightningCloudMaxSize);
        spawnedMissile2.GetComponent<Volt_LightningMissile>().SetMissileCloudInfo(lightningCloudDuration, lightningCloudGrowthRate, lightningCloudGrowthAmount, lightningCloudMaxSize);
        spawnedMissile3.GetComponent<Volt_LightningMissile>().SetMissileCloudInfo(lightningCloudDuration, lightningCloudGrowthRate, lightningCloudGrowthAmount, lightningCloudMaxSize);

        //rotate missile 2 and 3 to form a fan attack.
        spawnedMissile2.transform.Rotate(0, sideMissileOffset, 0);
        spawnedMissile3.transform.Rotate(0, -sideMissileOffset, 0);

    }

    public override void DeactivateAbility()
    {
        //do not use
    }
}

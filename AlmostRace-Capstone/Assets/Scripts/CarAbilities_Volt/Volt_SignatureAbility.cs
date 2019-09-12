using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Volt_SignatureAbility : Ability
{
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




    public override void ActivateAbility()
    {
        GameObject spawnedMissile1 = Instantiate(lightningMissile, missileSpawnLocation.transform.position, missileSpawnLocation.transform.rotation);
        GameObject spawnedMissile2 = Instantiate(lightningMissile, missileSpawnLocation.transform.position, missileSpawnLocation.transform.rotation);
        GameObject spawnedMissile3 = Instantiate(lightningMissile, missileSpawnLocation.transform.position, missileSpawnLocation.transform.rotation);

        spawnedMissile1.GetComponent<Volt_LightningMissile>().SetMissileInfo(missileExplosionDamage, missileFuseLength, missileSpeed, lightningCloudDuration, gameObject);
        spawnedMissile2.GetComponent<Volt_LightningMissile>().SetMissileInfo(missileExplosionDamage, missileFuseLength, missileSpeed, lightningCloudDuration, gameObject);
        spawnedMissile3.GetComponent<Volt_LightningMissile>().SetMissileInfo(missileExplosionDamage, missileFuseLength, missileSpeed, lightningCloudDuration, gameObject);

        spawnedMissile2.transform.Rotate(0, sideMissileOffset, 0);
        spawnedMissile2.transform.Rotate(0, -sideMissileOffset, 0);

    }

    public override void DeactivateAbility()
    {
        //do not use
    }
}

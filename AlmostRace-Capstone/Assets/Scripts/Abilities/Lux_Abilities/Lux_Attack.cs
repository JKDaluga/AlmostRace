using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Eddie Borissov
 * This code handles the Lux's attack ability. It fires out two disks that shoot lasers out of either side.
 
     */

public class Lux_Attack : Ability
{
    [Header("Laser Disk Variables")]
    [Space(30)]

    [Tooltip("Put laser disk prefab here.")]
    public GameObject trackingDart;

    [Tooltip("How fast the dart will fly.")]
    public float dartSpeed;

    [Tooltip("How much damage the laser from the disk will deal.")]
    public float laserDamage;

    [Tooltip("How often the laser deals damage per second.")]
    public float laserDamageRate;

    [Tooltip("How long the laser lasts.")]
    public float laserDuration;

    [Header("Spawning and Visuals variables")]
    [Space(30)]

    [Tooltip("Put the disk you want to go forward here.")]
    public Transform muzzle;

    public override void AbilityInUse()
    {

    }

    public override void AbilityOffOfCooldown()
    {

    }

    public override void AbilityOnCooldown()
    {

    }

    public override void ActivateAbility()
    {
        AudioManager.instance.Play("Lux Shooting", transform);

        GameObject spawnedDart = Instantiate(trackingDart, muzzle.position, muzzle.rotation);
        spawnedDart.GetComponent<Lux_TrackingDart>().SetProjectileInfo(0, dartSpeed, 0);
        spawnedDart.GetComponent<Lux_TrackingDart>().SetImmunePlayer(gameObject);
        spawnedDart.GetComponent<Lux_TrackingDart>().setDartInfo(laserDamage, laserDamageRate, laserDuration);
        

        #region Old Lux Attack
        ///spawn the disk that goes forward and pass all of the information it needs to it.
        //GameObject laserDiskRight = Instantiate(laserDisk, laserDiskSpawnRight.position, laserDiskSpawnRight.rotation);
        //laserDiskRight.GetComponent<Lux_LaserDisk>().SetProjectileInfo(diskDamage, laserDiskSpeed, diskHypeToGain);
        //laserDiskRight.GetComponent<Lux_LaserDisk>().SetImmunePlayer(gameObject);
        //laserDiskRight.GetComponent<Lux_LaserDisk>().SetDiskInfo(laserDamage, laserPulseRate, laserHypeToGain);

        ///spawn the disk that goes backward and pass all of the information it needs to it.
        //GameObject laserDiskLeft = Instantiate(laserDisk, laserDiskSpawnLeft.position, laserDiskSpawnLeft.rotation);
        //laserDiskLeft.GetComponent<Lux_LaserDisk>().SetProjectileInfo(diskDamage, laserDiskSpeed * -1, laserHypeToGain);
        //laserDiskLeft.GetComponent<Lux_LaserDisk>().SetImmunePlayer(gameObject);
        //laserDiskLeft.GetComponent<Lux_LaserDisk>().SetDiskInfo(laserDamage, laserPulseRate, diskHypeToGain);
        //Destroy(laserDiskLeft, 10);
        //Destroy(laserDiskRight, 10);
        #endregion
    }

    public override void DeactivateAbility()
    {
        //shouldn't be needed, as the cooldown stuff should be handled externally, I think...
    }
}

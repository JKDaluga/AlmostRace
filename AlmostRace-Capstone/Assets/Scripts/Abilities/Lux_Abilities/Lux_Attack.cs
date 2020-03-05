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

    [Tooltip("How much damage the laser from the disk will deal.")]
    public float laserDamage;

    [Tooltip("How often the laser deals damage per second.")]
    public float laserPulseRate;

    [Tooltip("How much damage the disk itself can do on impact.")]
    public float diskDamage;

    [Tooltip("How fast the laser disk will move.")]
    public float laserDiskSpeed;

    [Tooltip("How fast the laser will chase its target.")]
    public float laserTrackingSpeed;

    [Tooltip("How much hype is given per thing lasered.")]
    public float laserHypeToGain;

    [Tooltip("How much hype is given if the disk direct impacts a car.")]
    public float diskHypeToGain;

    [Header("Spawning and Visuals variables")]
    [Space(30)]

    [Tooltip("Put the disk you want to go backwards here.")]
    public Transform laserDiskSpawnLeft; //make sure the Z axis is pointing backwards on this one

    [Tooltip("Put the disk you want to go forward here.")]
    public Transform laserDiskSpawnRight;


    public override void ActivateAbility()
    {
        AudioManager.instance.Play("Lux Shooting", transform);

        GameObject trackingDartForward = Instantiate(trackingDart, laserDiskSpawnRight.position, laserDiskSpawnRight.rotation);
        trackingDartForward.GetComponent<Lux_TrackingDart>().SetProjectileInfo(diskDamage, laserDiskSpeed, laserHypeToGain);
        trackingDartForward.GetComponent<Lux_TrackingDart>().SetImmunePlayer(gameObject);
        trackingDartForward.GetComponent<Lux_TrackingDart>().setDartInfo(laserDamage, laserPulseRate, diskHypeToGain, laserTrackingSpeed);
        Destroy(trackingDartForward, 10);

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

    }

    public override void DeactivateAbility()
    {
        //shouldn't be needed, as the cooldown stuff should be handled externally, I think...
    }
}

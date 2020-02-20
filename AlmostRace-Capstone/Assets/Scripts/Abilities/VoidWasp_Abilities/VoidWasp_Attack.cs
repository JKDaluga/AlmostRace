using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    /*
    Author: Jake Velicer
    Purpose: Contains the behavior for the VoidWasp missile attack.
    In particular, when ActivateAbility is called on this script,
    this script will check how many targets are available to shoot at, equally
    distribute the amount of missiles allow to fire between them,
    then spawn homing missiles and set their particular variables.
    */

public class VoidWasp_Attack : Ability
{

    [Header("Projectile Values")]
    [Space(5)]
    public GameObject voidwaspProjectile;
    public Transform rocketSpawnPosition;
    [Tooltip("How much damage the missile does on impact.")] public float missileDamage;
    [Tooltip("How quickly each projectile moves.")] public float missileSpeed;
    [Tooltip("How many projectile it shoots in total")] public int projectileCount = 8;
    [Tooltip("How fast the projectile corrects its direction toward the target")] public float turnRate;
    [Tooltip("Wait time between each projectile fired in a sequence")] public float timeBetweenLaunch = 0.15f;
    [Tooltip("How long the projectile waits till it tracks")] public float hangTime;
    public float hypeToGain;
    private List<GameObject> _objectsInRange = new List<GameObject>();

    private Vector3 spawnOffset;

    public override void ActivateAbility()
    {
        if (_objectsInRange.Count > 0)
        {
            for (int i = 0; i < _objectsInRange.Count; i++)
            {
                if (_objectsInRange[i] == null)
                {
                    _objectsInRange.RemoveAt(i);
                }
                else
                {
                    StartCoroutine(LaunchSequence(_objectsInRange[i]));
                }
            }
        }
    }

    private IEnumerator LaunchSequence(GameObject target)
    {
        // Equally distribute the projectile count among the current targets
        int missileDistributionCount = projectileCount / _objectsInRange.Count;
        for (int j = 0; j < missileDistributionCount; j++)
        {
            // Spawn the missile at the spawn position and set its values
            spawnOffset = new Vector3(Random.Range(-2, 2), Random.Range(-2, 2), Random.Range(-2, 2));
            GameObject currentProjectile = Instantiate(voidwaspProjectile, rocketSpawnPosition.position + spawnOffset, rocketSpawnPosition.rotation);
            currentProjectile.GetComponent<VoidWasp_HomingMissile>().SetProjectileInfo(missileDamage, missileSpeed, hypeToGain);
            currentProjectile.GetComponent<VoidWasp_HomingMissile>().SetAdditionalInfo(target, turnRate, hangTime);
            currentProjectile.GetComponent<VoidWasp_HomingMissile>().SetImmunePlayer(gameObject);
            AudioManager.instance.Play("VoidWasp Shot");
            Destroy(currentProjectile, 7);
            yield return new WaitForSeconds(timeBetweenLaunch);
        }
    }

    public override void DeactivateAbility() {}

    public void AddObjectInRange(GameObject objectToAdd)
    {
        _objectsInRange.Add(objectToAdd);
    }

    public void RemoveObjectInRange(GameObject objectToRemove)
    {
        for(int i = _objectsInRange.Count - 1; i >= 0; i--)
        {
            if(_objectsInRange[i] == objectToRemove)
            {
                _objectsInRange.RemoveAt(i);
            }
        }
    }
}

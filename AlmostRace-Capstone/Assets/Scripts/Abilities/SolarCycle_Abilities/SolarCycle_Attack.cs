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

public class SolarCycle_Attack : Ability
{

    [Header("Projectile Values")]
    [Space(5)]
    public GameObject projectile;
    public Transform[] targetRocketSpawnPositions;
    public Transform[] staticRocketSpawnPositions;
    [Tooltip("How much damage the missile does on impact.")] public float missileDamage;
    [Tooltip("How quickly each projectile moves.")] public float missileSpeed;
    [Tooltip("How many projectile it shoots in total")] public int projectileCount = 8;
    [Tooltip("How fast the projectile corrects its direction toward the target")] public float turnRate;
    [Tooltip("Wait time between each projectile fired in a sequence")] public float timeBetweenLaunch = 0.15f;
    [Tooltip("How long the projectile waits till it tracks")] public float hangTime;
    [Tooltip("How long the projectile lives max")] public float maxLifeTime = 7;
    public Animator attackAnimator;
    public Animator[] engineAnimators;
    public float hypeToGain;
    private List<GameObject> _objectsInRange = new List<GameObject>();
    private Vector3 spawnOffset;

    public override void ActivateAbility()
    {
        int curentTargetCount = _objectsInRange.Count;
        int testForTargetMatchCount = 0;
        if (_objectsInRange.Count > 0)
        {
            for (int i = 0; i < _objectsInRange.Count; i++)
            {
                if (_objectsInRange[i] == null)
                {
                    _objectsInRange.RemoveAt(i);
                   testForTargetMatchCount = testForTargetMatchCount + 1;
                }
                else
                {
                    StartCoroutine(LaunchSequence(_objectsInRange[i]));
                }
            }
            if (curentTargetCount == testForTargetMatchCount)
            {
                StartCoroutine(LaunchSequence());
            }
        }
        else
        {
            StartCoroutine(LaunchSequence());
        }
    }

    private IEnumerator LaunchSequence(GameObject target)
    {
        // Equally distribute the projectile count among the current targets
        int missileDistributionCount = projectileCount / _objectsInRange.Count;
        int currentSpawnLocation = 0;
        for (int j = 0; j < missileDistributionCount; j++)
        {
            /* If we want to never have the missile spawn if the specific target is dead
            if (CheckTargetAlive(target) == false)
            {
                break;
            }
            */
            
            // Spawn the missile at the spawn position and set its values
            //spawnOffset = new Vector3(Random.Range(-2, 2), Random.Range(-2, 2), Random.Range(-.5f, .5f));
            GameObject currentProjectile = Instantiate(projectile, targetRocketSpawnPositions[currentSpawnLocation].position /*+ spawnOffset*/, targetRocketSpawnPositions[currentSpawnLocation].rotation);
            currentProjectile.GetComponent<SolarCycle_HomingMissile>().SetProjectileInfo(missileDamage, missileSpeed, hypeToGain);
            currentProjectile.GetComponent<SolarCycle_HomingMissile>().SetAdditionalInfo(target, turnRate, hangTime);
            currentProjectile.GetComponent<SolarCycle_HomingMissile>().SetImmunePlayer(gameObject);

            AudioManager.instance.Play("VoidWasp Shot", this.transform);
            Destroy(currentProjectile, maxLifeTime);
            yield return new WaitForSeconds(timeBetweenLaunch);
            if((currentSpawnLocation + 1) < (targetRocketSpawnPositions.Length - 1))
            {
                //makes sure we don't go out of bounds on our missile launcher muzzles
                currentSpawnLocation++;
            }
            else
            {
                //if we were about to go out of bounds, reset back to 0!
                currentSpawnLocation = 0;
            }
        }
        attackAnimator.SetTrigger("CoolDownStart");
        foreach (Animator engineAnimation in engineAnimators)
        {
            engineAnimation.Play("EngineDown");
        }
    }

    private IEnumerator LaunchSequence()
    {
        int currentSpawnLocation = 0;
        for (int j = 0; j < projectileCount; j++)
        {
            // Spawn the missile at the spawn position and set its values
            GameObject currentProjectile = Instantiate(projectile, staticRocketSpawnPositions[currentSpawnLocation].position, staticRocketSpawnPositions[currentSpawnLocation].rotation);
            currentProjectile.GetComponent<SolarCycle_HomingMissile>().SetProjectileInfo(missileDamage, missileSpeed, hypeToGain);
            currentProjectile.GetComponent<SolarCycle_HomingMissile>().SetAdditionalInfo(null, turnRate, 0);
            currentProjectile.GetComponent<SolarCycle_HomingMissile>().SetImmunePlayer(gameObject);

            AudioManager.instance.Play("VoidWasp Shot", this.transform);
            Destroy(currentProjectile, maxLifeTime);
            yield return new WaitForSeconds(timeBetweenLaunch);
            if((currentSpawnLocation + 1) < (staticRocketSpawnPositions.Length - 1))
            {
                //makes sure we don't go out of bounds on our missile launcher muzzles
                currentSpawnLocation++;
            }
            else
            {
                //if we were about to go out of bounds, reset back to 0!
                currentSpawnLocation = 0;
            }
        }
        attackAnimator.SetTrigger("CoolDownStart");
        foreach (Animator engineAnimation in engineAnimators)
        {
            engineAnimation.Play("EngineDown");
        }
    }

    private bool CheckTargetAlive(GameObject givenTarget)
    {
        if (givenTarget.GetComponent<CarHealthBehavior>() != null && givenTarget.GetComponent<CarHealthBehavior>().healthCurrent <= 0)
        {
            return false;
        }
        else if (givenTarget.GetComponent<Interactable>() != null && givenTarget.GetComponent<Interactable>().interactableHealth <= 0)
        {
            return false;
        }
        return true;
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

    public int ObjectCount()
    {
        return _objectsInRange.Count;
    }

    public override void AbilityOnCooldown()
    {

    }

    public override void AbilityOffOfCooldown()
    {
        attackAnimator.SetTrigger("CoolDownEnd");
    }

    public override void AbilityInUse()
    {
        attackAnimator.SetTrigger("AttackStart");
        foreach (Animator engineAnimation in engineAnimators)
        {
            engineAnimation.Play("EngineUp");
        }
    }
}

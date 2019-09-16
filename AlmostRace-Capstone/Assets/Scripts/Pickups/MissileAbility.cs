using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileAbility : Ability
{
    public GameObject missile;
    public Transform spawnLocation;

    public override void ActivateAbility()
    {
        if (spawnLocation == null)
        {
            spawnLocation = GameObject.Find("MissileSpawnLocation").transform;
        }
        GameObject missileInstance = Instantiate(missile, spawnLocation.position, spawnLocation.rotation);
        missileInstance.GetComponent<MissileBehavior>().SetImmunePlayer(gameObject);
    }
    public override void DeactivateAbility()
    {
        //not used
    }
}

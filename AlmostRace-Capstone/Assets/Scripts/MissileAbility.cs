using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileAbility : Ability
{
    public GameObject missile;
    public Transform spawnLocation;


    public override void ActivateAbility()
    {
        GameObject missileInstance = Instantiate(missile, spawnLocation.position, spawnLocation.rotation);
        missileInstance.GetComponent<MissileBehavior>().SetImmunePlayer(gameObject);
    }
    public override void DeactivateAbility()
    {
        //not used
    }
}

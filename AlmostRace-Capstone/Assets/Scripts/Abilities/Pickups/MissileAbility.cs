using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 Eddie B. UNUSED CODE
*/
public class MissileAbility : Ability
{
    public GameObject missile;
    public Transform spawnLocation;

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
        if (spawnLocation == null)
        {
            spawnLocation = GameObject.Find("MissleFireTransform").transform;
        }
        GameObject missileInstance = Instantiate(missile,
            new Vector3(spawnLocation.position.x, spawnLocation.position.y + 2, spawnLocation.position.z + 5),
            Quaternion.Euler(spawnLocation.rotation.x + 90, spawnLocation.rotation.y, spawnLocation.rotation.z));
        missileInstance.GetComponent<MissileBehavior>().SetImmunePlayer(gameObject);
    }
    public override void DeactivateAbility()
    {
        //not used
    }
}

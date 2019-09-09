using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileAbility : Ability
{
    public GameObject missile;
    public Transform spawnLocation;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public override void Fire()
    {
        GameObject missileInstance = Instantiate(missile, spawnLocation.position, spawnLocation.rotation);
        missileInstance.GetComponent<MissileBehavior>().SetImmunePlayer(gameObject);
    }
}

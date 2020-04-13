using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 
    Eddie B

    Ultra simple "player detection" that feeds the turret a single target at a time. 
    Can be expanded easily to feed the turret multiple targets and allow the turret to parse through them based on criteria.
    (For example, to look for HotSpotBot)
    
     */

public class TurretAggroBehavior : MonoBehaviour
{
    public TurretBehavior turret;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.GetComponent<CarHealthBehavior>() != null && turret.currentTarget == null)
        {//if other is a car and I'm not already shooting at someone
            turret.currentTarget = other.gameObject;
            turret.TriggerInteractable();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<CarHealthBehavior>() != null)
        {//if other is a car
            if (turret.currentTarget == other.gameObject)
            {
                turret.currentTarget = null;
            }
            
        }
    }
}

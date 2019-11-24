using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 
    Eddie B

    Ultra simple "player detection" that feeds the turret a single target at a time. 
    Can be expanded easily to feed the turret multiple targets and allow the turret to parse through them based on criteria.
    (For example, to look for HotSpotBot)
    
     */

public class TurretAggroBehavior2 : MonoBehaviour
{
    public TurretBehavior2 turret;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<CarHeatManager>() != null)
        {//if other is a car
            turret.possibleTargets.Add(other.gameObject);
            turret.TriggerInteractable();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<CarHeatManager>() != null)
        {//if other is a car

            if (other.gameObject == turret.currentTarget)
            {
                turret.currentTarget = null;
            }
            turret.possibleTargets.Remove(other.gameObject);
            if (turret.possibleTargets.Count == 0)
            {

                turret.ResetInteractable();
            }


        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    /*
    Author: Jake Velicer
    Purpose: Adds and removes potential target objects for the VoidWasp Attack script;
    */
public class SolarCycle_AttackTargetDetector : MonoBehaviour
{
    public SolarCycle_Attack attackScript;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent("CarHealthBehavior") || other.gameObject.GetComponent("Interactable"))
        {
           attackScript.AddObjectInRange(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent("CarHealthBehavior") || other.gameObject.GetComponent("Interactable"))
        {
            attackScript.RemoveObjectInRange(other.gameObject);
        }
    }
}

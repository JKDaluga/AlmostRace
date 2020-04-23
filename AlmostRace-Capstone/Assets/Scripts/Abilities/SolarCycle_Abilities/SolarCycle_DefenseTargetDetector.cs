using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SolarCycle_DefenseTargetDetector : MonoBehaviour
{
    public SolarCycle_Defensive defenseScript;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent("CarHealthBehavior"))
        {
           defenseScript.AddObjectInRange(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent("CarHealthBehavior"))
        {
            defenseScript.RemoveObjectInRange(other.gameObject);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoidWasp_DefenseTargetDetector : MonoBehaviour
{
    public VoidWasp_Defensive defenseScript;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent("CarHealthBehavior") || other.gameObject.GetComponent("Interactable"))
        {
           defenseScript.AddObjectInRange(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent("CarHealthBehavior") || other.gameObject.GetComponent("Interactable"))
        {
            defenseScript.RemoveObjectInRange(other.gameObject);
        }
    }
}

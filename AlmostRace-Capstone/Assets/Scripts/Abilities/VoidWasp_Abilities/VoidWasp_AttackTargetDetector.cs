using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoidWasp_AttackTargetDetector : MonoBehaviour
{
    public VoidWasp_Attack attackScript;

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

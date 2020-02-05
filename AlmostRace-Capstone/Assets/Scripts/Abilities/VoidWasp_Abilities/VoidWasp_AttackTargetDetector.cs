using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoidWasp_AttackTargetDetector : MonoBehaviour
{
    private List<GameObject> _objectsInRange = new List<GameObject>();

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent("CarHealthBehavior") || other.gameObject.GetComponent("Interactable"))
        {
            _objectsInRange.Add(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent("CarHealthBehavior") || other.gameObject.GetComponent("Interactable"))
        {
            for(int i = _objectsInRange.Count - 1; i >= 0; i--)
            {
                if(_objectsInRange[i] == other.gameObject)
                {
                    _objectsInRange.RemoveAt(i);
                }
            }
        }
    }
}

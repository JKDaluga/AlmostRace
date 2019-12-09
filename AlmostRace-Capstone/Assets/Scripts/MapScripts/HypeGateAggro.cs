using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 Eddie B

    Another simple detector that tracks cars that enter arena.
     */

public class HypeGateAggro : MonoBehaviour
{
    public HypeGateBehavior _hypeGate;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.GetComponent<VehicleHypeBehavior>() != null)
        {//if thing entering aggro is car
            if(!_hypeGate.carsInRange.Contains(other.gameObject)) //if car hasn't already been detected
            {
                if (_hypeGate.carsInRange.Count == 0)//if no cars were in range, start checking cars in range
                {//prevents the couroutine from having to be called in Start, saves performance
                    _hypeGate.InitializeHypeGate(gameObject);
                    _hypeGate.StartCoroutine(_hypeGate.CheckCars());
                }
                _hypeGate.carsInRange.Add(other.gameObject);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<VehicleHypeBehavior>() != null)
        {//if thing entering aggro is car
            if (_hypeGate.carsInRange.Contains(other.gameObject)) //if car hasn't already been detected
            {
                _hypeGate.carsInRange.Remove(other.gameObject);
            }
        }
    }

}

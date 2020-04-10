using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 Eddie B

    Another simple detector that tracks cars that enter arena.
     */

public class HypeGateAggro : MonoBehaviour
{
    public HypeGateTimeBehavior _hypeGate;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.GetComponent<RaycastCar>() != null)
        {//if thing entering aggro is car
            if(!_hypeGate.carsInRange.Contains(other.gameObject)) //if car hasn't already been detected
            {
                if (_hypeGate.carsInRange.Count == 0)//if no cars were in range, start checking cars in range
                {//prevents the couroutine from having to be called in Start, saves performance
                    _hypeGate.InitializeHypeGate(gameObject);
                    _hypeGate.isActivated = true;
                    _hypeGate.StartCoroutine(_hypeGate.CheckCars());
                }
                _hypeGate.carsInRange.Add(other.gameObject);
                int id_num = other.gameObject.GetComponent<RaycastCar>().playerID - 1;
                FindObjectOfType<DataManager>().playerInfo[id_num].timerRace1 = FindObjectOfType<RaceManager>().time;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<RaycastCar>() != null)
        {//if thing entering aggro is car
            if (_hypeGate.carsInRange.Contains(other.gameObject)) //if car hasn't already been detected
            {
                _hypeGate.carsInRange.Remove(other.gameObject);
            }
        }
    }

}

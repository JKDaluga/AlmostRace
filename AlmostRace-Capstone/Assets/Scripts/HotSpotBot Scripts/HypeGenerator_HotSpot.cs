using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HypeGenerator_HotSpot : MonoBehaviour
{
    private List<VehicleHypeBehavior> _vehiclesHyped;
    public float hypeGained = 1;
    public float hypeRate = 0.5f;
    // Start is called before the first frame update

    private void Start()
    {
        _vehiclesHyped = new List<VehicleHypeBehavior>();
        InvokeRepeating("GiveHype", 0, hypeRate);
    }

    private void GiveHype()
    {
        if(_vehiclesHyped.Count != 0)
        {
            foreach(VehicleHypeBehavior carsHyped in _vehiclesHyped)
            {
                carsHyped.AddHype(hypeGained);
            }
        }
        else
        {
            //this is if no cars are in the circle.
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Collided with: " + other.gameObject.name);
        if (other.gameObject.GetComponent<VehicleHypeBehavior>() != null)
        {
            Debug.Log("Hype Car Found!");
            _vehiclesHyped.Add(other.gameObject.GetComponent<VehicleHypeBehavior>());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("UNCollided with: " + other.gameObject.name);
        if (other.gameObject.GetComponent<VehicleHypeBehavior>() != null)
        {
            Debug.Log("Hype Car Lost!");
            _vehiclesHyped.Remove(other.gameObject.GetComponent<VehicleHypeBehavior>());
        }
    }
}

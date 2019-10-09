using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HypeGenerator_HotSpot : MonoBehaviour
{
    private List<VehicleHypeBehavior> _vehiclesHyped;
    public float hypeGained = 1;
    public float hypeRate = 0.5f;
    // Start is called before the first frame update

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.GetComponent<VehicleHypeBehavior>() != null)
        {
            _vehiclesHyped.Add(other.gameObject.GetComponent<VehicleHypeBehavior>());
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIAbilityBehaviour : MonoBehaviour
{

    public LayerMask targets, carTargets;
    VehicleAbilityBehavior fireButton;

    // Start is called before the first frame update
    void Start()
    {
        fireButton = GetComponent<VehicleAbilityBehavior>();
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (Physics.Raycast(transform.position, transform.forward, 100, targets))
        {
            fireButton.offensiveTrigger = true;
            print("Yes, We Fire");
            GetComponentInChildren<AIObstacleAvoidance>().avoiding = false;
            GetComponentInChildren<AIObstacleAvoidance>().turnL = false;
            GetComponentInChildren<AIObstacleAvoidance>().turnR = false;
            if (Physics.Raycast(transform.position, transform.forward, 100, carTargets))
            {
                fireButton.boostTrigger = true;
                
            }
        }
        else
        {
            fireButton.offensiveTrigger = false;
            fireButton.boostTrigger = false;
        }

    }

    
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIAbilityBehaviour : MonoBehaviour
{
    
    public LayerMask targets, carTargets;
    public AIObstacleAvoidance obstacleAvoidance;
    VehicleAbilityBehavior fireButton;
    RaycastHit targ;

    // Start is called before the first frame update
    void Start()
    {
        fireButton = GetComponent<VehicleAbilityBehavior>();
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (GetComponent<AIBehaviour>().canDrive)
        {
            if (GetComponent<VoidWasp_Attack>())
            {
                if(GetComponent<VoidWasp_Attack>().ObjectCount() > 0)
                {
                    fireButton.offensiveTrigger = true;
                    obstacleAvoidance.avoiding = false;
                    obstacleAvoidance.turnL = false;
                    obstacleAvoidance.turnR = false;
                    
                }
                else
                {
                    fireButton.offensiveTrigger = false;
                    
                }
                if (Physics.Raycast(transform.position, transform.forward, 100, carTargets))
                {
                    fireButton.boostTrigger = true;
                }
                else
                {
                    fireButton.boostTrigger = false;
                }
            }
            else
            {
                if (Physics.Raycast(transform.position, transform.forward, out targ, 100, targets))
                {
                    if (targ.collider.gameObject != gameObject)
                    {
                        fireButton.offensiveTrigger = true;
                        obstacleAvoidance.avoiding = false;
                        obstacleAvoidance.turnL = false;
                        obstacleAvoidance.turnR = false;
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

        }
    }

    
}

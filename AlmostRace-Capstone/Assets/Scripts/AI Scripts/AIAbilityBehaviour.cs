using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIAbilityBehaviour : MonoBehaviour
{
    
    public LayerMask targets, carTargets;
    public AIObstacleAvoidance obstacleAvoidance;
    VehicleAbilityBehavior fireButton;
    RaycastHit targ;

    private AIBehaviour _carHolder;
    private VoidWasp_Attack _attackHolder;
    private bool isVoidWasp = false;



    // Start is called before the first frame update
    void Start()
    {
        fireButton = GetComponent<VehicleAbilityBehavior>();

        _carHolder = GetComponent<AIBehaviour>();

        if (_carHolder.GetComponent<VoidWasp_Attack>() == true)
        {
            _attackHolder = _carHolder.GetComponent<VoidWasp_Attack>();
            isVoidWasp = true;
        }
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (_carHolder.canDrive)
        {
            if (isVoidWasp)
            {
                if(_attackHolder.ObjectCount() > 0)
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

    public static bool acquireToken()
    {
        if (RaceManager.tokens > 0)
        {
            RaceManager.tokens--;
            return true;
        }
        return false;
    }

    public static void returnToken()
    {
        RaceManager.tokens++;
    }
}

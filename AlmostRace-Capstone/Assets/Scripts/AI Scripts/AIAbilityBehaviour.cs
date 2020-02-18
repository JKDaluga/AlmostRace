using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIAbilityBehaviour : MonoBehaviour
{

    public LayerMask targets;
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
        }
        else
            fireButton.offensiveTrigger = false;

        //Distance Checker for boost abilities
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.GetMask("Abilities"))
        {
            if (this != other.gameObject.transform.root.GetComponent<Projectile>())
            {
                fireButton.defensiveTrigger = true;
                print("Defense is firing");
            }
                
        }
        fireButton.defensiveTrigger = false;
    }
}

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
        if (Physics.Raycast(transform.position, transform.right, 100, targets))
        {
            fireButton.offensiveTrigger = true;
        }
        else
            fireButton.offensiveTrigger = false;
    }
}

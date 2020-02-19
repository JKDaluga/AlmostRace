using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIShieldBehavior : MonoBehaviour
{
    float resetTimer = 1;

    private void FixedUpdate()
    {
        if (GetComponentInParent<VehicleAbilityBehavior>().defensiveTrigger)
        {
            float timer = 0;
            timer += Time.deltaTime;
            if(timer >= resetTimer)
            {
                GetComponentInParent<VehicleAbilityBehavior>().defensiveTrigger = false;
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Abilities"))
        {
            GetComponentInParent<VehicleAbilityBehavior>().defensiveTrigger = true;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncomingAttackDetection : MonoBehaviour
{
    public GameObject thisVehicle;
    public PlayerUIManager playerUIScript;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Abilities"))
        {
            if (other.gameObject.GetComponent<Projectile>() != null)
            {
                if(thisVehicle != other.gameObject.GetComponent<Projectile>().getImmunePlayer())
                {
                    playerUIScript.AddToAttacksInRange(other.gameObject.transform);
                }
            }
            else
            {
                playerUIScript.AddToAttacksInRange(other.gameObject.transform);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Abilities"))
        {
            RemoveAttackIndicator(other.gameObject);
        }
    }

    public void RemoveAttackIndicator(GameObject attackObject)
    {
        playerUIScript.RemoveAttackInRange(attackObject.transform);
        
    }
}

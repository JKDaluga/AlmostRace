using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleAbilityBehavior : MonoBehaviour
{
    [Header ("Basic Ability")]
    [Tooltip("Basic Ability Script Slot")] public Ability basicAbility;
    [Tooltip("The Button for using a Basic Ability")] public string basicAbilityInput;
    [Tooltip("Determines if the basic ability input can be held down")] public bool canHoldBasic;
    private bool canUseBasic = true;

    [Header ("Signature Ability")]
    [Tooltip("Signature Ability Script Slot")] public Ability signatureAbility;
    [Tooltip("Length of ability cooldown in seconds.")] public float abilityRecharge = 5f;
    [Tooltip("The Button for using a Signature Ability")] public string signatureAbilityInput;
    [Tooltip("Determines if the signature ability input can be held down")]public bool canHoldSignature;
    private bool canUseSignature = true;
    
    [Header ("Pickup Ability")]
    [Tooltip("Pickup Ability Script Slot")] public Ability pickup;
    [Tooltip("The Button for using Pickups")] public string pickupInput;


    // Update is called once per frame
    void Update()
    {
        // Basic Ability Call
        checkFireAbility(basicAbility, basicAbilityInput, canUseBasic, canHoldBasic);
        
        // Signature Ability Call
        if (checkFireAbility(signatureAbility, signatureAbilityInput, canUseSignature, canHoldSignature))
        {
            canUseSignature = false;
            StartCoroutine(AbilityCooldown());
        }

        // Pickup Ability Call
        if (Input.GetButtonDown(pickupInput) && pickup != null)
        {
            pickup.ActivateAbility();
            pickup = null;
        }
    }

    // Handles the ability call, on what input it is, if it can be used, and if it can be held down
    private bool checkFireAbility(Ability ability, string abilityInput, bool canFire, bool canHoldInput)
    {
        if (canHoldInput)
        {
            if (Input.GetButton(abilityInput) && canFire && ability != null)
            {
                ability.ActivateAbility();
                return true;
            }
            else if(Input.GetButtonUp(abilityInput) && ability != null)
            {
                ability.DeactivateAbility();
                return false; 
            }
        }
        else
        {
            if (Input.GetButtonDown(abilityInput) && canFire && ability != null)
            {
                ability.ActivateAbility();
                return true;
            }
        }
        return false;
    }

    private IEnumerator AbilityCooldown()
    {
        float tempTime = abilityRecharge;

        while (tempTime > 0)
        {
          //  Debug.Log(tempTime);
            tempTime -= Time.deltaTime;
            Mathf.Lerp(0, 1, tempTime);
            yield return null;
        }
        canUseSignature = true;
    }

    public bool hasPickup()
    {
        if (pickup != null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void assignPickup(Ability givenPickup)
    {
        pickup = givenPickup;
    }
}

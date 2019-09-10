using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleAbilityBehavior : MonoBehaviour
{
    [Header ("Basic Ability")]
    [Tooltip("Basic Ability Script Slot")] public Ability basicAbility;
    [Tooltip("The Button for using a Basic Ability")] public string basicAbilityInput;
    private bool canUseBasic = true;

    [Header ("Signature Ability")]
    [Tooltip("Signature Ability Script Slot")] public Ability signatureAbility;
    [Tooltip("Length of ability cooldown in seconds.")] public float abilityRecharge = 5f;
    [Tooltip("The Button for using a Signature Ability")] public string signatureAbilityInput;
    private bool canUseSignature = true;
    
    [Header ("Pickup Ability")]
    [Tooltip("Pickup Ability Script Slot")] public Ability pickup;
    [Tooltip("The Button for using Pickups")] public string pickupInput;

    public void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Input.GetButtonDown(basicAbilityInput) && canUseBasic && basicAbility != null)
        {
            basicAbility.Fire();
        }

        if (Input.GetButtonDown(signatureAbilityInput) && canUseSignature && signatureAbility != null)
        {
            signatureAbility.Fire();
            canUseSignature = false;
            StartCoroutine(AbilityCooldown()); 
        }

        if(Input.GetButtonDown(pickupInput) && pickup != null)
        {
            pickup.Fire();
            pickup = null;
        }

    }

    private IEnumerator AbilityCooldown()
    {

        float tempTime = abilityRecharge;

        while (tempTime > 0)
        {
            Debug.Log(tempTime);
            tempTime -= Time.deltaTime;
            Mathf.Lerp(0, 1, tempTime);
            yield return null;
        }

    }
}

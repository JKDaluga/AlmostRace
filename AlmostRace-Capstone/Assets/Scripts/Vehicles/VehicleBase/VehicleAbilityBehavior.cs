using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

    /*
    Author: Jake Velicer
    Purpose: Takes the plugged in scripts of the basic ability, signature ability,
    and runtime pickup abilites. Activates and deactivates those abilities based on user input.
    Controls when those abilities can be used.
    */

public class VehicleAbilityBehavior : MonoBehaviour
{
    [Header ("Basic Ability")]
    [Tooltip("Basic Ability Script Slot")]
    public BasicAbility basicAbility;
    [Tooltip("Determines if the basic ability input can be held down")]
    public bool canHoldBasic;
    private bool _canUseBasic = true;

    [Tooltip("Place UI element here.")]
    public Image basicAbilityButtonUI;
    [Tooltip("Place basic ability sprite 1 here.")]
    public Sprite basicAbilitySpriteReady;
    [Tooltip("Place basic ability sprite 2 here.")]
    public Sprite basicAbilitySpritePressed;

    [Header ("Signature Ability")]
    [Tooltip("Signature Ability Script Slot")]
    public Ability signatureAbility;
    [Tooltip("Length of ability cooldown in seconds.")]
    public float abilityRecharge = 5f;
    [Tooltip("Determines if the signature ability input can be held down")]
    public bool canHoldSignature;

    private bool _canUseSignature = true;

    [Tooltip("Place UI element here.")]
    public Image signatureAbilityCooldown;
    public GameObject signatureAbilityDark;



    [Header ("Pickup Ability")]
    [Tooltip("Pickup Ability Script Slot")]
    public Ability pickup;

    private VehicleInput _vehicleInput;

    private void Awake()
    {
        _vehicleInput = GetComponent<VehicleInput>();
        signatureAbilityDark.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (!_vehicleInput.getStatus())
        {
            return;
        }

        if(basicAbility != null && signatureAbility != null) //placed here just so that the BallCar prefab doesn't throw nulls
        {
            // Basic Ability Call
            checkFireAbility(basicAbility, _vehicleInput.basicAbilityInput, _canUseBasic, canHoldBasic);

            // Signature Ability Call
            if (checkFireAbility(signatureAbility, _vehicleInput.signatureAbilityInput, _canUseSignature, canHoldSignature))
            {
                _canUseSignature = false;
                StartCoroutine(AbilityCooldown());
            }
        }
   

        // Pickup Ability Call
        if (Input.GetButtonDown(_vehicleInput.pickupInput) && pickup != null)
        {
            pickup.ActivateAbility();
        }
    }

    // Handles the ability call, on what input it is, if it can be used, and if it can be held down
    private bool checkFireAbility(Ability ability, string abilityInput, bool canFire, bool canHoldInput)
    {
        if (canHoldInput) // currently ONLY used for basic
        {
            if (Input.GetButton(abilityInput) && canFire && ability != null)
            {
                basicAbilityButtonUI.sprite = basicAbilitySpritePressed;
                ability.ActivateAbility();
                return true;
            }
            else if (Input.GetButtonUp(abilityInput) && ability != null)
            {
                basicAbilityButtonUI.sprite = basicAbilitySpriteReady;
                ability.DeactivateAbility();
                return false;
            }
        }
        else // currently ONLY used for signature
        {
            if (Input.GetButtonDown(abilityInput) && canFire && ability != null)
            {
                signatureAbilityCooldown.fillAmount = 1;
                signatureAbilityDark.SetActive(true);
                ability.ActivateAbility();
                return true;
            }
 
        }
        return false;
    }

    // Countdown timer until reuse allowed for abilites that need a cooldown
    private IEnumerator AbilityCooldown()
    {
        float tempTime = abilityRecharge;

        while (tempTime > 0)
        {        
            tempTime -= Time.deltaTime;
            signatureAbilityCooldown.fillAmount = tempTime/abilityRecharge;
            yield return null;
        }
        signatureAbilityDark.SetActive(false);
        _canUseSignature = true;
    }

    // Returns if the pickup slot is vacant or occupied
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

    // Assigns the pickup slot with a given pickup
    public void assignPickup(GameObject givenPickup)
    {
        pickup = givenPickup.GetComponent<Ability>();
    }
}

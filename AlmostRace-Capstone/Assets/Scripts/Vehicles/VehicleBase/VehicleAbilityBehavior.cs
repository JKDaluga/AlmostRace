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
    [Header ("Offensive Ability.................................................................................")]
    [Tooltip("Offensive Ability Script Slot")]
    public HeatAbility offensiveAbility;
    [Tooltip("Determines if the Offensive ability input can be held down")]
    public bool canHoldBasic;
    private bool _canUseBasic = true;

    [Tooltip("Place UI element here.")]
    public Image offensiveAbilityButtonUI;
    [Tooltip("Place Offensive ability normal sprite here.")]
    public Sprite offensiveAbilitySpriteReady;
    [Tooltip("Place Offensive ability pressed down sprite here.")]
    public Sprite offensiveAbilitySpritePressed;

    [Header ("Defensive Ability.................................................................................")]
    [Tooltip("Defensive Ability Script Slot")]
    public Ability defensiveAbility;
    [Tooltip("Length of ability cooldown in seconds.")]
    public float defensiveAbilityRecharge = 5f;
    [Tooltip("Length of ability duration in seconds.")]
    public float defensiveAbilityDuration = 3f;
    [Tooltip("Determines if the signature ability input can be held down")]
    public bool canHoldDefensiveAbility;

    private bool _canUseDefensiveAbility = true;

    [Tooltip("Place UI element here.")]
    public Image defensiveAbilityCooldown;

    [Tooltip("Place dark version of UI element here.")]
    public GameObject defensiveAbilityDark;

    [Header ("Boost Ability.................................................................................")]
    [Tooltip("Boost Ability Script Slot")]
    public Ability boostAbility;
    [Tooltip("Length of ability cooldown in seconds.")]
    public float boostAbilityRecharge = 5f;
    [Tooltip("Length of ability duration in seconds.")]
    public float boostAbilityDuration = 3f;
    private bool _canBoost = true;
    [Tooltip("Place UI element here.")]
    public Image boostAbilityCooldown;

    [Tooltip("Place dark version of UI element here.")]
    public GameObject boostAbilityDark;

    private VehicleInput _vehicleInput;

    private void Awake()
    {
        _vehicleInput = GetComponent<VehicleInput>();
        defensiveAbilityDark.SetActive(false);
        boostAbilityDark.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (_vehicleInput != null)
        {
            if (!_vehicleInput.getStatus())
            {
                return;
            }


            if (offensiveAbility != null && defensiveAbility != null) //placed here just so that the BallCar prefab doesn't throw nulls
            {
                // Basic Ability Call
                checkFireAbility(offensiveAbility, _vehicleInput.basicAbilityInput, _canUseBasic, canHoldBasic);

                // Signature Ability Call
                if (checkFireAbility(defensiveAbility, _vehicleInput.signatureAbilityInput, _canUseDefensiveAbility, canHoldDefensiveAbility))
                {
                    _canUseDefensiveAbility = false;
                    StartCoroutine(DefensiveAbilityCooldown());
                    StartCoroutine(DefensiveAbilityDuration());
                }
            }


            // Boost Ability Call
            if (Input.GetButtonDown(_vehicleInput.pickupInput) && boostAbility != null)
            {
                if (_canBoost)
                {
                    _canBoost = true;
                    boostAbilityDark.SetActive(true);
                    boostAbility.ActivateAbility();
                    StartCoroutine(BoostAbilityDuration());
                    AudioManager.instance.Play("Boost");
                }

            }
        }
    }

    // Handles the ability call, on what input it is, if it can be used, and if it can be held down
    private bool checkFireAbility(Ability ability, string abilityInput, bool canFire, bool canHoldInput)
    {
        if (canHoldInput) // currently ONLY used for basic
        {
            if (Input.GetButton(abilityInput) && canFire && ability != null)
            {
                offensiveAbilityButtonUI.sprite = offensiveAbilitySpritePressed;
                ability.ActivateAbility();
                return true;
            }
            else if (Input.GetButtonUp(abilityInput) && ability != null)
            {
                offensiveAbilityButtonUI.sprite = offensiveAbilitySpriteReady;
                ability.DeactivateAbility();
                return false;
            }
        }
        else // currently ONLY used for signature
        {
            if (Input.GetButtonDown(abilityInput) && canFire && ability != null)
            {
                defensiveAbilityCooldown.fillAmount = 1;
                defensiveAbilityDark.SetActive(true);
                ability.ActivateAbility();
                return true;
            }
        }
        return false;
    }

    // Countdown timer until reuse allowed for abilites that need a cooldown
    private IEnumerator DefensiveAbilityCooldown()
    {
        float tempTime = defensiveAbilityRecharge;
        while (tempTime > 0)
        {        
            tempTime -= Time.deltaTime;
            defensiveAbilityCooldown.fillAmount = tempTime/defensiveAbilityRecharge;
            yield return null;
        }
        defensiveAbilityDark.SetActive(false);
        _canUseDefensiveAbility = true;
        AudioManager.instance.Play("Ability Recharge");
    }

    private IEnumerator DefensiveAbilityDuration()
    {
        float tempTime = defensiveAbilityDuration;
        while (tempTime > 0)
        {
            tempTime -= Time.deltaTime;
            yield return null;
        }
        defensiveAbility.DeactivateAbility();
    }

    private IEnumerator BoostAbilityCooldown()
    {
        float tempTime = boostAbilityRecharge;
        while (tempTime > 0)
        {
            tempTime -= Time.deltaTime;
            boostAbilityCooldown.fillAmount = tempTime / boostAbilityRecharge;
            yield return null;
        }
        boostAbilityDark.SetActive(false);
        _canBoost = true;
        AudioManager.instance.Play("Ability Recharge");
    }

    private IEnumerator BoostAbilityDuration()
    {
        _canBoost = false;
        float tempTime = boostAbilityDuration;
        while (tempTime > 0)
        {
            tempTime -= Time.deltaTime;
            yield return null;
        }
        boostAbility.DeactivateAbility();
        GetComponent<RaycastCar>().cheatPhysics();
    }

    // Assigns the pickup slot with a given pickup
    public void assignPickup(GameObject givenPickup)
    {
        boostAbility = givenPickup.GetComponent<Ability>();
    }
}

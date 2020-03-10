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
    public Ability offensiveAbility;
    [Tooltip("Length of ability cooldown in seconds.")]
    public float offensiveAbilityRecharge = 5f;
    [Tooltip("Determines if the Offensive ability input can be held down")]
    private bool _canUseBasic = true;

    [Tooltip("Place UI element here.")]
    public Image offensiveAbilityCooldown;
    [Tooltip("Place dark version of UI element here.")]
    public GameObject offensiveAbilityDark;

    [Header ("Defensive Ability.................................................................................")]
    [Tooltip("Defensive Ability Script Slot")]
    public Ability defensiveAbility;
    [Tooltip("Length of ability cooldown in seconds.")]
    public float defensiveAbilityRecharge = 5f;
    [Tooltip("Length of ability duration in seconds.")]
    public float defensiveAbilityDuration = 3f;

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
    [HideInInspector] public bool offensiveTrigger = false;
    [HideInInspector] public bool defensiveTrigger = false;
    [HideInInspector] public bool boostTrigger = false;
    private VehicleAwardsTracker tracker;

    [Header ("Ability Activator.................................................................................")]
    [Tooltip("Boolean that lets cars use their abilities")]
    public bool abilitiesActivated;

    private void Awake()
    {
        if (gameObject.GetComponent<VehicleInput>())
        {
            _vehicleInput = GetComponent<VehicleInput>();

            offensiveAbilityDark.SetActive(false);
            defensiveAbilityDark.SetActive(false);
            boostAbilityDark.SetActive(false);
            tracker = GetComponent<VehicleAwardsTracker>();
        }
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
        }
        getInput();

        // Basic Ability Call
        if (offensiveAbility != null && offensiveTrigger && abilitiesActivated) //placed here just so that the BallCar prefab doesn't throw nulls
        {
            if(fireAbility(offensiveAbility, _canUseBasic, offensiveAbilityCooldown, offensiveAbilityDark, 'o'))
            {
                _canUseBasic = false;
                offensiveAbility.AbilityInUse();
                StartCoroutine(OffensiveAbilityCooldown());
                
                
            }
        }

        // Signature Ability Call
        if (defensiveAbility != null && defensiveTrigger && abilitiesActivated)
        {
            if (fireAbility(defensiveAbility, _canUseDefensiveAbility, defensiveAbilityCooldown, defensiveAbilityDark, 'd'))
            {
                _canUseDefensiveAbility = false;
                defensiveAbility.AbilityInUse();
                StartCoroutine(DefensiveAbilityCooldown());
                StartCoroutine(DefensiveAbilityDuration());
            }
        }

        // Boost Ability Call
        if (boostAbility != null && boostTrigger && abilitiesActivated)
        {
            if (fireAbility(boostAbility, _canBoost, boostAbilityCooldown, boostAbilityDark, 'b'))
            {
                _canBoost = false;
                boostAbility.AbilityInUse();
                StartCoroutine(BoostAbilityCooldown());
                StartCoroutine(BoostAbilityDuration());
                GetComponent<RaycastCar>().isBoosting = true;
                AudioManager.instance.Play("Boost", transform);
            }

        }
        
    }

    // Handles the ability call, on what input it is, if it can be used, and if it can be held down
    private bool fireAbility(Ability ability, bool canFire, Image abilityCooldown, GameObject abilityDark, char flagChar)
    {
        if (canFire && ability != null)
        {
            if (_vehicleInput != null)
            {
                abilityCooldown.fillAmount = 1;
                abilityDark.SetActive(true);
                tracker.awardUpdate(flagChar, _vehicleInput.getPlayerNum());
            }
            ability.ActivateAbility();
            
            return true;
        }
        return false;
    }

    // Countdown timer until reuse allowed for abilites that need a cooldown
    private IEnumerator OffensiveAbilityCooldown()
    {
        float tempTime = offensiveAbilityRecharge;
        while (tempTime > 0)
        {
            tempTime -= Time.deltaTime;
            if (_vehicleInput != null)
            {
                offensiveAbilityCooldown.fillAmount = tempTime / offensiveAbilityRecharge;
            }
            yield return null;
        }
        if (_vehicleInput != null)
        {
            offensiveAbilityDark.SetActive(false);
            AudioManager.instance.PlayWithoutSpatial("Ability Recharge");
        }
        _canUseBasic = true;
        offensiveAbility.AbilityOffOfCooldown();
    }

    // Countdown timer until reuse allowed for abilites that need a cooldown
    private IEnumerator DefensiveAbilityCooldown()
    {
        float tempTime = defensiveAbilityRecharge;
        while (tempTime > 0)
        {        
            tempTime -= Time.deltaTime;
            if (_vehicleInput != null)
            {
                defensiveAbilityCooldown.fillAmount = tempTime / defensiveAbilityRecharge;
            }
            yield return null;
        }
        if (_vehicleInput != null)
        {
            defensiveAbilityDark.SetActive(false);
            AudioManager.instance.PlayWithoutSpatial("Ability Recharge");
        }
        _canUseDefensiveAbility = true;
        defensiveAbility.AbilityOffOfCooldown();
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
        defensiveAbility.AbilityOnCooldown();
    }

    private IEnumerator BoostAbilityCooldown()
    {
        float tempTime = boostAbilityRecharge;
        while (tempTime > 0)
        {
            tempTime -= Time.deltaTime;
            if (_vehicleInput != null)
            {
                boostAbilityCooldown.fillAmount = tempTime / boostAbilityRecharge;
            }
            yield return null;
        }
        if (_vehicleInput != null)
        {
            boostAbilityDark.SetActive(false);
            AudioManager.instance.PlayWithoutSpatial("Ability Recharge");
        }
        _canBoost = true;
        boostAbility.AbilityOffOfCooldown();
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
        boostAbility.AbilityOnCooldown();
        GetComponent<RaycastCar>().cheatPhysics();

        GetComponent<RaycastCar>().isBoosting = false;
    }

    private void getInput()
    {
        if(_vehicleInput != null)
        {
            if (!_vehicleInput.getStatus())
            {
                offensiveTrigger = false;
                defensiveTrigger = false;
                boostTrigger = false;
            }
            else
            {
                offensiveTrigger = Input.GetButtonDown(_vehicleInput.basicAbilityInput);
                defensiveTrigger = Input.GetButtonDown(_vehicleInput.signatureAbilityInput);
                boostTrigger = Input.GetButtonDown(_vehicleInput.pickupInput);
            }
        }
    }

    public void Activation()
    {
        abilitiesActivated = true;
        print("Abilities on");
    }
}

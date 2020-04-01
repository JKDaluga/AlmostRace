using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;

    /*
    Author: Jake Velicer
    Purpose: Takes the plugged in scripts of the basic ability, signature ability,
    and runtime pickup abilites. Activates and deactivates those abilities based on user input.
    Controls when those abilities can be used.
    */

public class VehicleAbilityBehavior : MonoBehaviour
{
    public RaycastCar rayCastCar;

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
    [Tooltip("Place Background Fill of UI element here.")]
    public GameObject offensiveAbilityBG;

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


    [Tooltip("Place Background Fill of UI element here.")]
    public GameObject DefensiveAbilityBG;

    [Header ("Boost Ability.................................................................................")]
    [Tooltip("Boost Ability Script Slot")]
    public Ability boostAbility;
    [Tooltip("Length of ability cooldown in seconds.")]
    public float boostAbilityRecharge = 5f;
    [Tooltip("Length of ability duration in seconds.")]
    public float boostAbilityDuration = 3f;
    private bool _canBoost = true;
    //public CinemachineVirtualCamera cineCamera;
   // public float carFovStandard = 50f;
   // public float carFovBoost = 80f;
   // public float carFovChangeAmount = .1f;
   // public float carFovChangeRate = .1f;
    [Tooltip("Place UI element here.")]
    public Image boostAbilityCooldown;

    [Tooltip("Place dark version of UI element here.")]
    public GameObject boostAbilityDark;

    [Tooltip("Place Background Fill of UI element here.")]
    public GameObject BoostAbilityBG;

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

            //rayCastCar = GetComponent<RaycastCar>();
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
            if(fireAbility(offensiveAbility, _canUseBasic, offensiveAbilityCooldown, offensiveAbilityDark, offensiveAbilityBG, 'o'))
            {
                _canUseBasic = false;
                offensiveAbility.AbilityInUse();
                StartCoroutine(OffensiveAbilityCooldown());              
            }
        }

        // Signature Ability Call
        if (defensiveAbility != null && defensiveTrigger && abilitiesActivated)
        {
            if (fireAbility(defensiveAbility, _canUseDefensiveAbility, defensiveAbilityCooldown, defensiveAbilityDark, DefensiveAbilityBG, 'd'))
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
            if (fireAbility(boostAbility, _canBoost, boostAbilityCooldown, boostAbilityDark, BoostAbilityBG, 'b'))
            {
                _canBoost = false;
                boostAbility.AbilityInUse(); 
                StartCoroutine(BoostAbilityCooldown());
                StartCoroutine(BoostAbilityDuration());
                rayCastCar.isBoosting = true;
            //    StartCoroutine(ChangeFOV());
                AudioManager.instance.Play("Boost", transform);
            }

        }
        
    }

    /*private IEnumerator ChangeFOV()
    {
       // Debug.Log("Change FOV was called!");
        while(true)
        {
            if(rayCastCar.isBoosting == true)
            {//you are boosting
               // Debug.Log("Car is boosting!");
               if(_vehicleInput != null)
                { // if human player
                    if (cineCamera.m_Lens.FieldOfView < carFovBoost)
                    {
                        //Debug.Log("Car FOV was increased from: " + cineCamera.m_Lens.FieldOfView);
                        cineCamera.m_Lens.FieldOfView += carFovChangeAmount;
                        // Debug.Log(" to: " + cineCamera.m_Lens.FieldOfView);
                    }
                    else
                    {
                        cineCamera.m_Lens.FieldOfView = carFovBoost;
                        yield break;
                    }
                }
               
            }
            if(rayCastCar.isBoosting == false)
            {
                if (_vehicleInput != null)
                { // if human player
                    if (cineCamera.m_Lens.FieldOfView > carFovStandard)
                    {
                        cineCamera.m_Lens.FieldOfView -= carFovChangeAmount;
                    }
                    else
                    {
                        cineCamera.m_Lens.FieldOfView = carFovStandard;
                        yield break;
                    }
                }
            }
            yield return new WaitForSeconds(carFovChangeRate);
        }
    }*/

    // Handles the ability call, on what input it is, if it can be used, and if it can be held down
    private bool fireAbility(Ability ability, bool canFire, Image abilityCooldown, GameObject abilityDark, GameObject abilityBG, char flagChar)
    {
        if (canFire && ability != null)
        {
            if (_vehicleInput != null)
            {
                abilityCooldown.fillAmount = 1;
                abilityDark.SetActive(true);
                abilityBG.SetActive(true);
                StartCoroutine(FillReset(abilityBG));
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
        float tempTime = 0;
        while (tempTime < offensiveAbilityRecharge)
        {
            tempTime += Time.deltaTime;
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
        float tempTime = 0;
        while (tempTime < defensiveAbilityRecharge)
        {        
            tempTime += Time.deltaTime;
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
        float tempTime = 0;
        while (tempTime < boostAbilityRecharge)
        {
            tempTime += Time.deltaTime;
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
        rayCastCar.cheatPhysics();
        rayCastCar.isBoosting = false;
        //StartCoroutine(ChangeFOV());
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


    private IEnumerator FillReset(GameObject abilityBG)
    {
        yield return new WaitForSeconds(.1f);

        abilityBG.SetActive(false);
    }
    public void Activation()
    {
        abilitiesActivated = true;
        //print("Abilities on");
    }
}

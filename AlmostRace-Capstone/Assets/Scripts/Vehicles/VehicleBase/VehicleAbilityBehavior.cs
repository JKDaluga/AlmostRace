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

    [Tooltip("Place UI element here.")] public Image offensiveFillBar;

    [Tooltip("Place dark version of UI element here.")] public GameObject offensiveBrightIcon;

    [Tooltip("Place dark version of UI element here.")] public GameObject offensiveBrightHex;

    [Tooltip("Place Background Fill of UI element here.")] public GameObject offensiveAbilityBG;

    [Header ("Defensive Ability.................................................................................")]

    [Tooltip("Defensive Ability Script Slot")] public Ability defensiveAbility;

    [Tooltip("Length of ability cooldown in seconds.")] public float defensiveAbilityRecharge = 5f;

    [Tooltip("Length of ability duration in seconds.")] public float defensiveAbilityDuration = 3f;

    private bool _canUseDefensiveAbility = true;

    [Tooltip("Place UI element here.")] public Image defensiveAbilityFillAmount;

    [Tooltip("Place bright icon here.")] public GameObject defensiveBrightIcon;

    [Tooltip("Place bright hex here.")] public GameObject defensiveBrightHex;

    [Tooltip("Place Background Fill of UI element here.")] public GameObject DefensiveAbilityBG;

    [Header ("Boost Ability.................................................................................")]

    [Tooltip("Boost Ability Script Slot")] public Ability boostAbility;

    [Tooltip("Length of ability cooldown in seconds.")] public float boostAbilityRecharge = 5f;

    [Tooltip("Length of ability duration in seconds.")]public float boostAbilityDuration = 3f;

    private bool _canBoost = true;
    //public CinemachineVirtualCamera cineCamera;
   // public float carFovStandard = 50f;
   // public float carFovBoost = 80f;
   // public float carFovChangeAmount = .1f;
   // public float carFovChangeRate = .1f;
    [Tooltip("Place UI element here.")] public Image boostFillBar;

    [Tooltip("Place bright icon here.")] public GameObject boostBrightIcon;

    [Tooltip("Place bright hex here.")] public GameObject boostBrightHex;

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

    private bool isAI = false;

    private void Awake()
    {
        if (gameObject.GetComponent<VehicleInput>())
        {
            _vehicleInput = GetComponent<VehicleInput>();

            offensiveBrightIcon.SetActive(false);
            defensiveBrightIcon.SetActive(false);
            boostBrightIcon.SetActive(false);
            tracker = GetComponent<VehicleAwardsTracker>();
        }
        else
        {
            isAI = true;
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
        if (offensiveAbility != null && offensiveTrigger && abilitiesActivated) 
        {
            if(fireAbility(offensiveAbility, _canUseBasic, offensiveFillBar, offensiveBrightIcon, offensiveAbilityBG, 'o'))
            {
                _canUseBasic = false;
                offensiveAbility.AbilityInUse();
                StartCoroutine(OffensiveAbilityCooldown());              
            }
        }

        // Signature Ability Call
        if (defensiveAbility != null && defensiveTrigger && abilitiesActivated)
        {
            if (fireAbility(defensiveAbility, _canUseDefensiveAbility, defensiveAbilityFillAmount, defensiveBrightIcon, DefensiveAbilityBG, 'd'))
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
            if (fireAbility(boostAbility, _canBoost, boostFillBar, boostBrightIcon, BoostAbilityBG, 'b'))
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
        if (canFire && ability != null && (!isAI || AIAbilityBehaviour.acquireToken()))
        {
            if (_vehicleInput != null)
            {
                abilityCooldown.fillAmount = 1;
                abilityDark.SetActive(true);
                abilityBG.SetActive(true);
                StartCoroutine(FillReset(abilityBG));
                tracker.awardUpdate(flagChar, _vehicleInput.getPlayerNum());
            }
            else
            {
                Invoke("returnToken", RaceManager.tokenRefreshTime);
            }
            ability.ActivateAbility();

            return true;

        }
        return false;
    }

    // Countdown timer until reuse allowed for abilites that need a cooldown
    private IEnumerator OffensiveAbilityCooldown()
    {
        if (_vehicleInput != null)
        {//ability is back on cooldown
            offensiveBrightIcon.SetActive(false);
            offensiveBrightHex.SetActive(false);
        }
        float tempTime = 0;
        while (tempTime < offensiveAbilityRecharge)
        {
            tempTime += Time.deltaTime;
            if (_vehicleInput != null)
            {
                offensiveFillBar.fillAmount = tempTime / offensiveAbilityRecharge;
            }
            yield return null;
        }

        if (_vehicleInput != null)
        {//ability is back online
            offensiveBrightIcon.SetActive(true);
            offensiveBrightHex.SetActive(true);
            AudioManager.instance.PlayWithoutSpatial("Ability Recharge");
        }
        _canUseBasic = true;
        offensiveAbility.AbilityOffOfCooldown();
    }

    // Countdown timer until reuse allowed for abilites that need a cooldown
    private IEnumerator DefensiveAbilityCooldown()
    {
        if (_vehicleInput != null)
        {//ability is back on cooldown, update UI
            defensiveBrightIcon.SetActive(false);
            defensiveBrightHex.SetActive(false);
        }

        float tempTime = 0;
        while (tempTime < defensiveAbilityRecharge)
        {        
            tempTime += Time.deltaTime;
            if (_vehicleInput != null)
            {
                defensiveAbilityFillAmount.fillAmount = tempTime / defensiveAbilityRecharge;
            }
            yield return null;
        }

        if (_vehicleInput != null)
        {//ability is back online, update UI
            defensiveBrightIcon.SetActive(true);
            defensiveBrightHex.SetActive(true);
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
        if (_vehicleInput != null)
        {//ability is on cooldown, update UI
            boostBrightIcon.SetActive(false);
            boostBrightHex.SetActive(false);
        }

        float tempTime = 0;
        while (tempTime < boostAbilityRecharge)
        {
            tempTime += Time.deltaTime;
            if (_vehicleInput != null)
            {
                boostFillBar.fillAmount = tempTime / boostAbilityRecharge;
            }
            yield return null;
        }

        if (_vehicleInput != null)
        {//ability is back online, update UI
            boostBrightIcon.SetActive(true);
            boostBrightHex.SetActive(true);
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


    public void ResetAbilityAnims()
    {
        offensiveAbility.AbilityOffOfCooldown();
        defensiveAbility.AbilityOffOfCooldown();
        boostAbility.AbilityOffOfCooldown();
    }

    private IEnumerator FillReset(GameObject abilityBG)
    {
        yield return new WaitForSeconds(.1f);

        abilityBG.SetActive(false);
    }
    public void Activation()
    {
        abilitiesActivated = true;
    }

    public void returnToken()
    {
        AIAbilityBehaviour.returnToken();
    }
}

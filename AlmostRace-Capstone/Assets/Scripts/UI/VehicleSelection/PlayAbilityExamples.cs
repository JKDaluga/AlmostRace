using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayAbilityExamples : MonoBehaviour
{
    public Ability offensiveAbility;
    public float offensiveAbilityRecharge = 5f;
    public Ability defensiveAbility;
    public float defensiveAbilityRecharge = 5f;
    public float defensiveAbilityDuration = 3f;
    public Ability boostAbility;
    public float boostAbilityRecharge = 5f;
    public float boostAbilityDuration = 3f;
    public bool _canUseBasic;
    public bool _canUseDefensiveAbility;
    public bool _canBoost;
    
    void Update()
    {

        // Basic Ability Call
        if (offensiveAbility != null && _canUseBasic) 
        {
            if(fireAbility(offensiveAbility, _canUseBasic))
            {
                _canUseBasic = false;
                offensiveAbility.AbilityInUse();
                StartCoroutine(OffensiveAbilityCooldown());
            }
        }

        // Signature Ability Call
        if (defensiveAbility != null && _canUseDefensiveAbility)
        {
            if (fireAbility(defensiveAbility, _canUseDefensiveAbility))
            {
                _canUseDefensiveAbility = false;
                defensiveAbility.AbilityInUse();
                StartCoroutine(DefensiveAbilityCooldown());
                StartCoroutine(DefensiveAbilityDuration());
            }
        }

        // Boost Ability Call
        if (boostAbility != null && _canBoost)
        {
            if (fireAbility(boostAbility, _canBoost))
            {
                _canBoost = false;
                boostAbility.AbilityInUse(); 
                StartCoroutine(BoostAbilityCooldown());
                StartCoroutine(BoostAbilityDuration());
            }

        }
    }

    private bool fireAbility(Ability ability, bool canFire)
    {
        if (canFire && ability != null)
        {
            ability.ActivateAbility();
            return true;

        }
        return false;
    }


    private IEnumerator OffensiveAbilityCooldown()
    {
        float tempTime = 0;
        while (tempTime < offensiveAbilityRecharge)
        {
            tempTime += Time.deltaTime;
            yield return null;
        }
        offensiveAbility.AbilityOffOfCooldown();
        tempTime = 0;
        while (tempTime < offensiveAbilityRecharge / 2)
        {
            tempTime += Time.deltaTime;
            yield return null;
        }        
        _canUseBasic = true;
    }

    // Countdown timer until reuse allowed for abilites that need a cooldown
    private IEnumerator DefensiveAbilityCooldown()
    {
        float tempTime = 0;
        while (tempTime < defensiveAbilityRecharge)
        {        
            tempTime += Time.deltaTime;
            yield return null;
        }
        defensiveAbility.AbilityOffOfCooldown();
        tempTime = 0;
        while (tempTime < defensiveAbilityRecharge / 2)
        {        
            tempTime += Time.deltaTime;
            yield return null;
        }
        _canUseDefensiveAbility = true;
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
            yield return null;
        }
        boostAbility.AbilityOffOfCooldown();
        tempTime = 0;
        while (tempTime < boostAbilityRecharge / 2)
        {
            tempTime += Time.deltaTime;
            yield return null;
        }
        _canBoost = true;
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
    }
}

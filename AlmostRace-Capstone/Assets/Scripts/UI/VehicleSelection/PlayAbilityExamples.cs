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
    public bool[] currentAbilityExample = new bool[4];
    private bool _canUseBasic = true;
    private bool _canUseDefensiveAbility = true;
    private bool _canBoost = true;
    
    private void Update()
    {
        if (offensiveAbility != null && _canUseBasic && currentAbilityExample[1]) 
        {
            if(fireAbility(offensiveAbility, _canUseBasic))
            {
                _canUseBasic = false;
                offensiveAbility.AbilityInUse();
                StartCoroutine(OffensiveAbilityCooldown());
            }
        }
        else if (defensiveAbility != null && _canUseDefensiveAbility && currentAbilityExample[2])
        {
            if (fireAbility(defensiveAbility, _canUseDefensiveAbility))
            {
                _canUseDefensiveAbility = false;
                defensiveAbility.AbilityInUse();
                StartCoroutine(DefensiveAbilityCooldown());
                StartCoroutine(DefensiveAbilityDuration());
            }
        }
        else if (boostAbility != null && _canBoost && currentAbilityExample[3])
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

    public void DeactivateCurrentAbility()
    {
        for (int i = 0; i < currentAbilityExample.Length; i++)
        {
            if (currentAbilityExample[i] == true)
            {
                if (i == 1)
                {
                    offensiveAbility.DeactivateAbility();
                }
                else if (i == 2)
                {
                    defensiveAbility.DeactivateAbility();                    
                }
                else if (i == 3)
                {
                    boostAbility.DeactivateAbility();
                }
            }
        }
    }

    public void DeactivateAllAbilites()
    {
        for (int i = 0; i < currentAbilityExample.Length; i++)
        {
            currentAbilityExample[i] = false;
            if (i == 1)
            {
                offensiveAbility.DeactivateAbility();
            }
            else if (i == 2)
            {
                defensiveAbility.DeactivateAbility();                    
            }
            else if (i == 3)
            {
                boostAbility.DeactivateAbility();
            }
        }
    }
}

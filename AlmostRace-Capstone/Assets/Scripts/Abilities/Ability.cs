using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    Creator and developer of script: Jason Daluga
    Purpose: Abstract Classes for abilities to inherit from 
*/
public abstract class Ability : MonoBehaviour
{
    public abstract void ActivateAbility();
    public abstract void DeactivateAbility();
    protected RaycastCar car;

    protected void Initialize()
    {
        car = gameObject.GetComponent<RaycastCar>();
    }
}

public abstract class CooldownAbility : Ability
{
    //protected float cooldown;
   // protected float duration;
}

public abstract class HeatAbility : Ability
{
    protected CarHealthBehavior carHeatInfo;
    [Tooltip("How much damage is done to the player every X seconds")] public float selfHeatDamage;
    protected abstract void AddHeat();
}

public abstract class CooldownHeatAbility : Ability
{
    protected CarHealthBehavior carHeatInfo;
    [Tooltip("How much damage is done to the player every X seconds")] public float selfHeatDamage;
    protected abstract void AddHeat();
    protected float cooldown;
    protected float duration;
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Ability : MonoBehaviour
{
    public abstract void ActivateAbility();
    public abstract void DeactivateAbility();
    protected VehicleHypeBehavior vehicleHypeScript;

    protected void Initialize()
    {
        vehicleHypeScript = gameObject.GetComponent<VehicleHypeBehavior>();
    }
}

public abstract class CooldownAbility : Ability
{
    //protected float cooldown;
   // protected float duration;
}

public abstract class HeatAbility : Ability
{
    protected CarHeatManager carHeatInfo;
    [Tooltip("How much damage is done to the player every X seconds")] public float selfHeatDamage;
    protected abstract void AddHeat();
}

public abstract class CooldownHeatAbility : Ability
{
    protected CarHeatManager carHeatInfo;
    [Tooltip("How much damage is done to the player every X seconds")] public float selfHeatDamage;
    protected abstract void AddHeat();
    protected float cooldown;
    protected float duration;
}

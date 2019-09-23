using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Ability : MonoBehaviour
{
    public abstract void ActivateAbility();
    public abstract void DeactivateAbility();
}

public abstract class BasicAbility : Ability
{
    public CarHeatManager carHeatInfo;
    [Tooltip("How much damage is done to the player every X seconds")] public float selfHeatDamage;
    protected abstract void AddHeat();
}

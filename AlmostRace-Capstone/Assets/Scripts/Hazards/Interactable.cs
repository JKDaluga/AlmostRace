using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 Eddie B

    An abstract class meant to be used by all interactables in maps (boulders, turrets, coolant tubes, etc).
     */
public abstract class Interactable : MonoBehaviour
{
    /// <summary>
    /// A method that can be used for triggering specific behavior on interactables.
    /// </summary>
    public abstract void TriggerInteractable();

    /// <summary>
    /// A method that can be used for triggering specific behavior on interactables.
    /// </summary>
    public abstract void DestroyInteractable();

    /// <summary>
    /// A method that can be used for resetting specific behavior on interactables.
    /// </summary>
    public abstract void ResetInteractable();

    /// <summary>
    /// A method that can deal damage to an interactable.
    /// </summary>
    /// <param name="damageNumber"> Place damage number here. </param>
    public abstract void DamageInteractable(float damageNumber);

    /// <summary>
    /// A bool for whether or not the interactable can take damage.
    /// </summary>
    public bool canBeDamaged;

    /// <summary>
    /// Interactable health value.
    /// </summary>
    public float interactableHealth;

    public GameObject interactingPlayer;
}

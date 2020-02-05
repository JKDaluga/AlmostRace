using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Health : MonoBehaviour
{
    /* Eddie Borissov 
     This abstract class is meant to be inherited by anything that has health.
     This inheritance will make code that interacts with health much easier and cleaner to write!
     */


    public float healthCurrent;
    public float healthMax;

    public abstract void Kill();

    public abstract void DamageHealth(float damageAmount);
    public abstract void DamageHealthTrue(float trueDamageAmount);
    public abstract float GetHealth();
    public abstract void SetHealth(float newHealth);
    public abstract void AddHealth(float healthToAdd);


}

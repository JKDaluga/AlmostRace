using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Eddie B

public class ShockRod : Interactable
{
    public float shockDamage;

    public float shockRate;

    public float shockDuration;

    public GameObject shockSphere;

    private void Start()
    {
        shockSphere.GetComponent<ShockRodSphere>().GiveInfo(shockDamage, shockRate);
    }

    public override void DamageInteractable(float damageNumber)
    {//currently interactables are simply triggered, so damageNumber can be ignored
        if(canBeDamaged)
        {
            canBeDamaged = false;
            TriggerInteractable();
        }
      
    }

    public override void DestroyInteractable()
    {
     //   throw new System.NotImplementedException();
    }

    public override void ResetInteractable()
    {
        canBeDamaged = true;
        shockSphere.SetActive(false);
    }

    public override void TriggerInteractable()
    {
        shockSphere.SetActive(true);
        Invoke("ResetInteractable", shockDuration);
    }


}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//eddie

public class Lux_EnergyBlade : Projectile, IPooledObject
{
    public int lifeTime = 7;
    private List<GameObject> _immuneCars;
    private Interactable hitInteractable;
    private float _growthRate;
    private float _growthAmount;
    private float _growthLimit;
    private CarHealthBehavior carHit;
    private string poolTag = "LuxAttack";
    private Vector3 startingScale;

    public void OnObjectActivate()
    {
        
    }

    public void OnObjectDeactivate()
    {
        CancelInvoke("Grow");
        transform.localScale = startingScale;
    }

    public void GiveInfo(float growthRate, float growthAmount, float growthLimit)
    {
        _growthRate = growthRate;
        _growthAmount = growthAmount;
        _growthLimit = transform.localScale.x + growthLimit;
        startingScale = transform.localScale;
        _immuneCars = new List<GameObject>();
        GiveSpeed();
        InvokeRepeating("Grow", 0, _growthRate);
        StartCoroutine(ObjectPooler.instance.DeactivateAfterTime(poolTag, gameObject, lifeTime));
    }

    public void Grow()
    {
        if(transform.localScale.x < _growthLimit)
        {
            transform.localScale += new Vector3(_growthAmount, 0, _growthAmount);
        }
        else
        {
            CancelInvoke("Grow");
        }
     
    }

    public void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Vehicle") && _immunePlayer != null && other.gameObject != _immunePlayer && !_immuneCars.Contains(other.gameObject))
        {
            carHit = other.gameObject.GetComponent<CarHealthBehavior>();
            carHit.DamageCar(_projectileDamage, _immunePlayerScript.playerID);
            _immuneCars.Add(other.gameObject);
            ObjectPooler.instance.Deactivate(poolTag, gameObject);
        }
        if(other.CompareTag("Interactable"))
        {
            hitInteractable = other.GetComponent<Interactable>();

            hitInteractable.DamageInteractable(hitInteractable.interactableHealth);
        }
    }


}

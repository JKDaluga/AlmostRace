using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    /*
    Author: Jake Velicer
    Purpose: Contains the behavior for the Void Wasp shield.
    In particular, when this script's activate ability is called, the shield is started.
    Any enemy vehicles that that exist in objects in range are damaged over time.
    At the same time, that same amount of health is applied to the players shields.
    */

public class SolarCycle_Defensive : CooldownAbility
{
    public float shieldHealth;
    public float siphonAmount;
    public float siphonFrequency = 1;
    public float minSimSpeed = 0.6f;
    public float maxSimSpeed = 1;
    public List<GameObject> _shields;
    private CarHealthBehavior _carHealthScript;
    private List<GameObject> _objectsInRange = new List<GameObject>();
    private float[] _startParticleLifetimes;
 
    void Start()
    {
        _carHealthScript = gameObject.GetComponent<CarHealthBehavior>();
        _startParticleLifetimes = new float[_shields.Count];
        for (int i = 0; i < _shields.Count; i++)
        {
            _startParticleLifetimes[i] = _shields[i].GetComponent<ParticleSystem>().main.startLifetime.constant;
        }
    }

    public override void ActivateAbility()
    {
        AudioManager.instance.Play("Shield Activated", transform);
        _carHealthScript.SetPersonalShieldAmount(shieldHealth);
        foreach (GameObject shield in _shields)
        {
            shield.GetComponent<ParticleSystem>().Play();
            ChangeSimSpeed(shield, _carHealthScript.GetPersonalShieldAmount());
        }
        StartCoroutine(ShieldEvent());
    }

    private IEnumerator ShieldEvent()
    {
        while(true)
        {
            if (_objectsInRange.Count > 0)
            {
                for (int i = 0; i < _objectsInRange.Count; i++)
                {
                    if (_objectsInRange[i] == null)
                    {
                        _objectsInRange.RemoveAt(i);
                    }
                    else
                    {
                        if (_objectsInRange[i] != null && _objectsInRange[i].GetComponent<CarHealthBehavior>() != null)
                        {
                            if(!_objectsInRange[i].GetComponent<CarHealthBehavior>().isDead)
                            {
                                _objectsInRange[i].GetComponent<CarHealthBehavior>().DamageCar(siphonAmount, _carHealthScript.raycastCarHolder.playerID);
                                if (_carHealthScript.GetPersonalShieldAmount() < _carHealthScript.GetExtaHealthMax())
                                {
                                    _carHealthScript.AddPersonalShields(siphonAmount);
                                }
                            }
                    
                        }
                        else
                        {
                            _objectsInRange.RemoveAt(i);
                        }
                    }
                }
            }
            ActiveShieldParticleChanges();
            yield return new WaitForSeconds(siphonFrequency);
        }
    }

    public override void DeactivateAbility()
    {
        _carHealthScript.SetPersonalShieldAmount(0);
        StopAllCoroutines();
        foreach (GameObject shield in _shields)
        {
            shield.GetComponent<ParticleSystem>().Stop();
            shield.GetComponent<ParticleSystem>().Clear();
        }
    }

    public void AddObjectInRange(GameObject objectToAdd)
    {
        _objectsInRange.Add(objectToAdd);
    }

    public void RemoveObjectInRange(GameObject objectToRemove)
    {
        for(int i = _objectsInRange.Count - 1; i >= 0; i--)
        {
            if(_objectsInRange[i] == objectToRemove)
            {
                _objectsInRange.RemoveAt(i);
            }
        }
    }

    private void ActiveShieldParticleChanges()
    {
        for (int i = 0; i < _shields.Count; i++)
        {
            var shieldsMain = _shields[i].GetComponent<ParticleSystem>().main;
            //shieldsMain.startLifetime = _startParticleLifetimes[i];
            ChangeSimSpeed(_shields[i], _carHealthScript.GetPersonalShieldAmount());
        }
    }

    private void ChangeSimSpeed(GameObject givenShield, float givenAmount)
    {
        var em = givenShield.GetComponent<ParticleSystem>().main;
        em.simulationSpeed = Mathf.Lerp(minSimSpeed, maxSimSpeed, (Mathf.Clamp(givenAmount, 0, 100)) / 100);
    }

    public override void AbilityOnCooldown()
    {

    }

    public override void AbilityOffOfCooldown()
    {

    }

    public override void AbilityInUse()
    {

    }
}

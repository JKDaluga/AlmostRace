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

public class VoidWasp_Defensive : CooldownAbility
{
    public float shieldHealth;
    public float siphonAmount;
    public float siphonFrequency = 1;
    public float minParticleCount = 20;
    public float maxParticleCount = 50;
    public List<GameObject> _shields;
    private CarHealthBehavior _carHealthScript;
    private List<GameObject> _objectsInRange = new List<GameObject>();
 
    void Start()
    {
        _carHealthScript = gameObject.GetComponent<CarHealthBehavior>();
        foreach(GameObject shield in _shields)
        {
            shield.GetComponent<ParticleSystem>().Stop();
        }
    }

    public override void ActivateAbility()
    {
        AudioManager.instance.Play("Shield Activated", transform);
        _carHealthScript.SetExtraHealth(shieldHealth);
        foreach (GameObject shield in _shields)
        {
            shield.GetComponent<ParticleSystem>().Play();
            ChangeEmissionCount(shield, _carHealthScript.GetExtraHealth());
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
                        if (!_objectsInRange[i].GetComponent<CarHealthBehavior>().isDead &&_objectsInRange[i] != null)
                        {
                            _objectsInRange[i].GetComponent<CarHealthBehavior>().DamageCar(siphonAmount);
                            if ( _carHealthScript.GetExtraHealth() < _carHealthScript.GetExtaHealthMax())
                            {
                                _carHealthScript.AddExtraHealth(siphonAmount);
                            }
                        }
                        else
                        {
                            _objectsInRange.RemoveAt(i);
                        }
                    }
                }
            }
            foreach (GameObject shield in _shields)
            {
                ChangeEmissionCount(shield, _carHealthScript.GetExtraHealth());
            }
            yield return new WaitForSeconds(siphonFrequency);
        }
    }

    public override void DeactivateAbility()
    {
        _carHealthScript.SetExtraHealth(0);
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

    private void ChangeEmissionCount(GameObject givenShield, float givenAmount)
    {
        var em = givenShield.GetComponent<ParticleSystem>().emission;
        em.rateOverTime = Mathf.Lerp(minParticleCount, maxParticleCount, (Mathf.Clamp(givenAmount, 0, 100)) / 100);
    }
}

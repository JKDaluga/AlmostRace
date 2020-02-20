using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoidWasp_Defensive : CooldownAbility
{
    public float shieldHealth;
    public float siphonAmount;
    public float siphonFrequency = 1;
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
        AudioManager.instance.Play("Shield Activated");
        _carHealthScript.SetExtraHealth(shieldHealth);
        foreach (GameObject shield in _shields)
        {
            shield.GetComponent<ParticleSystem>().Play();
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
                        _objectsInRange[i].GetComponent<CarHealthBehavior>().DamageCar(siphonAmount);
                        _carHealthScript.AddExtraHealth(siphonAmount);
                    }
                }
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
}

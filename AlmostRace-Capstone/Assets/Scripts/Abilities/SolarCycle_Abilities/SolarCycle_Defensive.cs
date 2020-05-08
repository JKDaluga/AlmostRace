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
    [Header("Ability Values")]
    public float shieldHealth;
    public float siphonAmount;
    public float siphonFrequency = 1;
    public float minSimSpeed = 0.6f;
    public float maxSimSpeed = 1;
    public Collider defenseDetectionCollider;
    private List<GameObject> _objectsInRange = new List<GameObject>();

    [Header("Effects Values")]
    public List<GameObject> shields;
    public List<Animator> shieldGenerators;
    public LineRenderer[] lineRender;
    public string explodeVFXTag = "SCExplodeVFX";
    private CarHealthBehavior _carHealthScript;
    private ObjectPooler _objectPooler;
    private float[] _startParticleLifetimes;
    private bool _isActive;
 
    void Start()
    {
        _carHealthScript = gameObject.GetComponent<CarHealthBehavior>();
        _objectPooler = ObjectPooler.instance;
        _startParticleLifetimes = new float[shields.Count];
        for (int i = 0; i < shields.Count; i++)
        {
            _startParticleLifetimes[i] = shields[i].GetComponent<ParticleSystem>().main.startLifetime.constant;
        }
    }

    public override void ActivateAbility()
    {
        AudioManager.instance.Play("Shield Activated", transform);
        _isActive = true;
        if (_carHealthScript != null)
        {
            _carHealthScript.SetPersonalShieldAmount(shieldHealth);
        }
        foreach (GameObject shield in shields)
        {
            shield.GetComponent<ParticleSystem>().Play();
            if (_carHealthScript != null)
            {
                ChangeSimSpeed(shield);
            }
        }
        foreach (Animator shieldGen in shieldGenerators)
        {
            shieldGen.Play("SCShieldGenActive");
        }
        if (_carHealthScript != null)
        {
            StartCoroutine(ShieldEvent());
        }
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
                        //Debug.Log("Removed Vehicle Via First Check: " + _objectsInRange[i].transform.parent);
                        if (i < lineRender.Length)
                        {
                            lineRender[i].SetPosition(0, transform.position);
                            lineRender[i].SetPosition(1, transform.position);
                        }
                    }
                    else if (!defenseDetectionCollider.bounds.Contains(_objectsInRange[i].transform.position))
                    {
                       RemoveObjectInRange(_objectsInRange[i]);
                    }
                    else
                    {
                        if (_objectsInRange[i] != null && _objectsInRange[i].GetComponent<CarHealthBehavior>() != null)
                        {
                            if(!_objectsInRange[i].GetComponent<CarHealthBehavior>().isDead)
                            {
                                if (i < lineRender.Length)
                                {
                                    lineRender[i].SetPosition(0, transform.position);
                                    lineRender[i].SetPosition(1, _objectsInRange[i].transform.position);
                                }
                                _objectsInRange[i].GetComponent<CarHealthBehavior>().DamageCar(siphonAmount, _carHealthScript.raycastCarHolder.playerID);
                                if (_carHealthScript.GetPersonalShieldAmount() < _carHealthScript.GetExtaHealthMax())
                                {
                                    _carHealthScript.AddPersonalShields(siphonAmount);
                                }
                            }
                            else
                            {
                                if (i < lineRender.Length)
                                {
                                    lineRender[i].SetPosition(0, transform.position);
                                    lineRender[i].SetPosition(1, transform.position);
                                }
                            }
                        }
                        else
                        {
                            _objectsInRange.RemoveAt(i);
                            //Debug.Log("Removed Vehicle Via Second Check: " + _objectsInRange[i].transform.parent);
                            if (i < lineRender.Length)
                            {
                                lineRender[i].SetPosition(0, transform.position);
                                lineRender[i].SetPosition(1, transform.position);
                            }
                        }
                    }
                }

            }
            else
            {
                for(int j = 0; j < lineRender.Length; j++)
                {
                    lineRender[j].SetPosition(0, transform.position);
                    lineRender[j].SetPosition(1, transform.position);
                }
            }
            ActiveShieldParticleChanges();
            yield return new WaitForSeconds(siphonFrequency);
        }
    }

    public override void DeactivateAbility()
    {
        if (_carHealthScript != null)
        {
            _carHealthScript.SetPersonalShieldAmount(0);
        }
        StopAllCoroutines();
        for(int j = 0; j < lineRender.Length; j++)
        {
            lineRender[j].SetPosition(0, transform.position);
            lineRender[j].SetPosition(1, transform.position);
        }
        foreach (GameObject shield in shields)
        {
            shield.GetComponent<ParticleSystem>().Stop();
            shield.GetComponent<ParticleSystem>().Clear();
        }
        foreach (Animator shieldGen in shieldGenerators)
        {
            shieldGen.Play("SCShieldGenCooldown");
            GameObject explodeVFXObject = _objectPooler.SpawnFromPool(explodeVFXTag, shieldGen.transform.position, shieldGen.transform.rotation);
            _objectPooler.StartCoroutine(_objectPooler.DeactivateAfterTime(explodeVFXTag, explodeVFXObject, 1));
            AudioManager.instance.Play("VoidWasp Companion Death", transform);
        }
        _isActive = false;
    }

    public void AddObjectInRange(GameObject objectToAdd)
    {
        if (!_objectsInRange.Contains(objectToAdd.gameObject))
        {
            //Debug.Log("Recieved Vehicle: " + objectToAdd.transform.parent);
            _objectsInRange.Add(objectToAdd);
        }
    }

    public void RemoveObjectInRange(GameObject objectToRemove)
    {
        for(int i = _objectsInRange.Count - 1; i >= 0; i--)
        {
            if(_objectsInRange[i] == objectToRemove)
            {
                //Debug.Log("Removed Vehicle Via Function: " + _objectsInRange[i].transform.parent);
                _objectsInRange.RemoveAt(i);
            }
        }
    }

    private void ActiveShieldParticleChanges()
    {
        for (int i = 0; i < shields.Count; i++)
        {
            var shieldsMain = shields[i].GetComponent<ParticleSystem>().main;
            ChangeSimSpeed(shields[i]);
        }
    }

    public void ChangeSimSpeed(GameObject givenShield)
    {
        if (_carHealthScript != null)
        {
            var em = givenShield.GetComponent<ParticleSystem>().main;
            em.simulationSpeed = Mathf.Lerp(minSimSpeed, maxSimSpeed, (Mathf.Clamp(_carHealthScript.GetPersonalShieldAmount(), 0, 100)) / 100);            
        }
    }

    public override void AbilityOnCooldown()
    {

    }

    public override void AbilityOffOfCooldown()
    {
        foreach (Animator shieldGen in shieldGenerators)
        {
            shieldGen.Play("SCShieldGenIdle");
        }
    }

    public override void AbilityInUse()
    {

    }

    public bool GetActive()
    {
        return _isActive;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PainTrain_ShieldShock : MonoBehaviour
{
    private List<CarHealthBehavior> _carsToDamage;
    private CarHealthBehavior _playerPainTrainScript;
    private GameObject _immunePlayer;
    private float currentHealth;
    private float newHealth;
    private float _zapCoolDown = .75f;
    private float _zapDamage;
    private float countDownTimer;
    private int randomTarget;

    private void Start()
    {
        _carsToDamage = new List<CarHealthBehavior>();
    }

    public void GiveInfo(float zapDamage, float zapCoolDown, GameObject immunePlayer)
    {
        _zapDamage = zapDamage;
        _zapCoolDown = zapCoolDown;
        _immunePlayer = immunePlayer;

        _playerPainTrainScript = _immunePlayer.GetComponent<CarHealthBehavior>();
    }


    public void TurnOnShieldShock()
    {
        _playerPainTrainScript = _immunePlayer.GetComponent<CarHealthBehavior>();
        StartCoroutine(TrackHealth());
    }

    public void TurnOffShieldShock()
    {
        _carsToDamage.Clear();

        StopAllCoroutines();
    }

    public void Zap()
    {
        //Debug.Log("Zap should have happened!");
        if(_carsToDamage.Count > 0)
        {
            //Debug.Log("Zap part 2 should have happened!");
            randomTarget = Random.Range(0, _carsToDamage.Count - 1);
            if (_carsToDamage[randomTarget].healthCurrent > 0)
            {
                Debug.Log("Car should have been damaged!");
                _carsToDamage[randomTarget].DamageCar(_zapDamage, _playerPainTrainScript.raycastCarHolder.playerID);
                if(_carsToDamage[randomTarget].healthCurrent < 0)
                {
                    _carsToDamage.RemoveAt(randomTarget);
                }
            }
        }

    }

    private IEnumerator TrackHealth()
    {
        currentHealth = _playerPainTrainScript.healthCurrent;
        while (true)
        {
            if(countDownTimer > 0)
            {
                countDownTimer -= Time.deltaTime;
            }
            newHealth = _playerPainTrainScript.healthCurrent;

            if(newHealth < currentHealth)
            {
                currentHealth = newHealth;
                if(countDownTimer <= 0)
                {
                    countDownTimer = _zapCoolDown;
                    Zap();
                }              
            }
            else
            {
                currentHealth = newHealth;
            }

            yield return null;
        }
      
    }


    public void AddCarToDamage(CarHealthBehavior carToAdd)
    {
        if (carToAdd != _immunePlayer)
        {
            if (!_carsToDamage.Contains(carToAdd))
            {
                _carsToDamage.Add(carToAdd);
            }
        }
    }

    public void RemoveCarFromDamage(CarHealthBehavior carToRemove)
    {
        if (carToRemove != _immunePlayer)
        {
            if (_carsToDamage.Contains(carToRemove))
            {
                _carsToDamage.Remove(carToRemove);
            }
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Vehicle") && other != _immunePlayer)
        {
            AddCarToDamage(other.GetComponent<CarHealthBehavior>());
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Vehicle") && other != _immunePlayer)
        {
            RemoveCarFromDamage(other.GetComponent<CarHealthBehavior>());
        }
    }
}

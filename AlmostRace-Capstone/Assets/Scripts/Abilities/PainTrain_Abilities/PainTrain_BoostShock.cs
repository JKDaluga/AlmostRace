using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Eddie and Greg

public class PainTrain_BoostShock : MonoBehaviour
{


    private Interactable hitInteractable;

    private List<CarHealthBehavior> _carsToDamage;

    private GameObject _immunePlayer;
    private CarHealthBehavior _carHealthScript;

    private float _shockDamage;

    private float _shockRate;

    // Start is called before the first frame update
    void Start()
    {
        _carHealthScript = _immunePlayer.GetComponent<CarHealthBehavior>();
        _carsToDamage = new List<CarHealthBehavior>();
    }

    public void GiveInfo(GameObject immunePlayer, float shockDamage, float shockRate)
    {
        _immunePlayer = immunePlayer;
        _shockDamage = shockDamage;
        _shockRate = shockRate;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Vehicle") && other.gameObject != _immunePlayer)
        {
            _carsToDamage.Add(other.GetComponent<CarHealthBehavior>());

            if (_carsToDamage.Count == 1)
            {
                InvokeRepeating("DamageCars", 0, _shockRate);
            }
        }
        else if (other.gameObject.GetComponent<Interactable>() != null)
        {
            hitInteractable = other.gameObject.GetComponent<Interactable>();
            hitInteractable.DamageInteractable(hitInteractable.interactableHealth);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<CarHealthBehavior>() != null && _carsToDamage.Contains(other.gameObject.GetComponent<CarHealthBehavior>()))
        {
            _carsToDamage.Remove(other.gameObject.GetComponent<CarHealthBehavior>());
        }
    }

    public void ClearCarList()
    {
        _carsToDamage.Clear();
        CancelInvoke("DamageCars");
    }

    void DamageCars()
    {
        if (_carsToDamage.Count != 0)
        {
            for(int i = _carsToDamage.Count - 1; i >= 0; i--)
            {
                if (!_carsToDamage[i].isDead)
                {
                    _carsToDamage[i].DamageCar(_shockDamage, _carHealthScript.raycastCarHolder.playerID);
                    if (_carsToDamage[i].healthCurrent <= 0)
                    {
                        _carsToDamage.Remove(_carsToDamage[i]);
                    }
                }
            }
        }
        else
        {
            CancelInvoke("DamageCars");
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Volt_LightningCone : MonoBehaviour
{
    /*
     Damage value and frequency values are sent from the ability script,
     that way designers only need to set those values in one script that is on the car,
     as opposed to an external object like the Lightning Cone.
     */

    private List<CarHeatManager> _carsBeingDamagedList;
    private bool isDamagingPlayers = false; //might not be necessary, depending on how often InvokeRepeating is getting called.
    private float _damagePerTick;
    private float _damageFrequency;
    private float _selfDamage;
    private GameObject _immunePlayer;


    private void Start()
    {
        _carsBeingDamagedList = new List<CarHeatManager>();
    }

    public void SetImmunePlayer(GameObject player)
    {
        _immunePlayer = player;
    }

    public void SetLightningDamageAndFrequency(float damage, float selfDamage, float frequency)
    {
        _damagePerTick = damage;
        _damageFrequency = frequency;
        _selfDamage = selfDamage;
    }

    private void DamageEnemies()
    {
        
        if(_carsBeingDamagedList.Count != 0)
        {
            isDamagingPlayers = true;
            foreach (CarHeatManager carHeat in _carsBeingDamagedList)
            {
                carHeat.heatCurrent -= _damagePerTick;
            }
        }
        else if(_carsBeingDamagedList.Count == 0)
        {
            isDamagingPlayers = false;
            CancelInvoke("DamageEnemies");
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject != _immunePlayer) //Prevents self damage.
        {
            Debug.Log(other.gameObject.name + " was hit by the lightning!");
            if(other.gameObject.GetComponent<CarHeatManager>() != null) //makes sure attack is hitting a car. Separated IFs for readability.
            {
                //Might have to add additional checks to allow environments to be damaged, not just cars.
                _carsBeingDamagedList.Add(other.gameObject.GetComponent<CarHeatManager>());
                if(!isDamagingPlayers)
                {
                    InvokeRepeating("DamageEnemies", 0, _damageFrequency);
                } 
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.GetComponent<CarHeatManager>() != null) //makes sure it's a car (or object with health, down the line).
        {
            if(_carsBeingDamagedList.Contains(other.gameObject.GetComponent<CarHeatManager>()))
            {
                _carsBeingDamagedList.Remove(other.gameObject.GetComponent<CarHeatManager>());
            }
        }
    }

    public void EndAbility()
    {
        _carsBeingDamagedList.Clear();
        CancelInvoke();
    }

}

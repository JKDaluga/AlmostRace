using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 Eddie B

 A relatively simple script to allow the walls of flame (this object) that are spawned from destroying the coolant
 tubes to damage multiple cars at once. This objects damage, rate of damage, and duration are set in the object
 that spawns them, to make life easier for developers and give them a centralized object to tweak values in.
     */

public class CoolantLineBehavior : MonoBehaviour
{
    private float _duration;
    private float _coolantDamage;
    private float _coolantDamageRate;
    private float _fireKillHype;
    private float _fireDamageHype;
    private Collider _coolantCollider;
    private GameObject _interactingPlayer;//the player that activated this object initially.
    public ParticleSystem fireWallParticle;
    private List<CarHealthBehavior> _carsDamaged;

    public void Start()
    {
        _coolantCollider = gameObject.GetComponent<Collider>();
        _carsDamaged = new List<CarHealthBehavior>();
        DeactivateCoolantLine();
    }

    /// <summary>
    /// Gives the coolant line all of the info it needs to function properly.
    /// </summary>
    /// <param name="duration">How long the coolant line remains on fire</param>
    /// <param name="coolantDamage">How much damage the coolant line does when active.</param>
    /// <param name="coolantDamageRate">How often damage is dealt to cars.</param>
    public void InitializeCoolantLine(float duration, float coolantDamage, float coolantDamageRate, float fireDamageHype, float fireKillHype)
    {
        _fireKillHype = fireKillHype;
        _fireDamageHype = fireDamageHype;
        _duration = duration;
        _coolantDamage = coolantDamage;
        _coolantDamageRate = coolantDamageRate;
    }

    /// <summary>
    /// Activates the coolant line and makes it deadly.
    /// </summary>
    public void ActivateCoolantLine(GameObject interactingPlayer)
    {
        _interactingPlayer = interactingPlayer;
        fireWallParticle.Play();
        _coolantCollider.enabled = true;
        Invoke("DeactivateCoolantLine", _duration);
        AudioManager.instance.Play("Cooling Rod Fire", transform);
    }

    public void DeactivateCoolantLine()
    {
        _carsDamaged.Clear();
        fireWallParticle.Stop();
        _coolantCollider.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.GetComponent<CarHealthBehavior>() != null)
        {//Checks if a car passes through. 
            //Debug.Log("Car Detected by fire!");
            _carsDamaged.Add(other.gameObject.GetComponent<CarHealthBehavior>()); //Adds car to damage.
            if(_carsDamaged.Count == 1)
            {//If this is the first car added to the list, assume that cars aren't being damaged, start the invoke repeating.
               
                InvokeRepeating("DamageCars", 0, _coolantDamageRate);
                // todo: @Eddie, add damage to interactables
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<CarHealthBehavior>() != null && _carsDamaged.Contains(other.gameObject.GetComponent<CarHealthBehavior>()))
        {//Checks if a car leaves the trigger and if it was already in the list.
            _carsDamaged.Remove(other.gameObject.GetComponent<CarHealthBehavior>()); //Removes car from.
            
            //Stopping the invoke repeating is handled internally in the DamageCars() function;
        }
    }

    private void DamageCars() //Damages any and all cars inside of the fire wall.
    {
        if(_carsDamaged.Count > 0)//Make sure _carsDamaged isn't empty.
        {
            foreach (CarHealthBehavior car in _carsDamaged)
            {
                if(!car.isDead) //Make sure car is alive.
                {
                    car.DamageCar(_coolantDamage, 100); //Damage car.
                    if(_interactingPlayer != null)
                    {
                        if (car.healthCurrent <= 0) //See if car was killed by the damage.
                        {
                            if (!_interactingPlayer.Equals(car.gameObject))
                            {
                                //rewards instigating player with hype for killing other cars
                                DataManager.instance.playerInfo[_interactingPlayer.GetComponent<RaycastCar>().playerID - 1].numKills++;
                            }
                            _carsDamaged.Remove(car);// If a car is killed, remove it from the list of cars being damaged.
                        }
                    }

                }
            }
        }
        else
        {//If no there are no cars to damage, save resources by canceling the invoke repeating.
            CancelInvoke("DamageCars");
        }
    }
}

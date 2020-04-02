using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ActivationEvent : UnityEvent<GameObject>
{

}

public class ActivateAbilities : MonoBehaviour
{
    public ActivationEvent _ability;
    private RaycastCar _carToAdd;
    private void Start()
    {
        if(_ability == null)
        {
            _ability = new ActivationEvent();
        }

        _ability.AddListener(AbilitiesOn);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<VehicleAbilityBehavior>())
        {
            _ability.Invoke(other.gameObject);
            if(other.GetComponent<VehicleInput>() != null)
            {//if it's a player
                _carToAdd = other.gameObject.GetComponent<RaycastCar>();
                    _carToAdd.playerUIManagerScript.weaponsActivatedText.SetActive(true);
                    _carToAdd.playerUIManagerScript.ResetWeaponText(4);
        
            }        
        }
    }


    void AbilitiesOn(GameObject car)
    {
        car.GetComponent<VehicleAbilityBehavior>().Activation();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupBehavior : MonoBehaviour
{
    public enum PickUps{misslePickUp };
    public PickUps chosenPickUp;
    private GameObject pickUp;

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Vehicle"))
        {
            if(collision.gameObject.GetComponent<VehicleAbilityBehavior>().hasPickup() == false)
            {
                if (chosenPickUp == PickUps.misslePickUp)
                {
                    pickUp = Resources.Load<GameObject>("MissileAbilityContainer");
                    collision.gameObject.GetComponent<VehicleAbilityBehavior>().assignPickup(pickUp);
                }
            }
        }
    }
}

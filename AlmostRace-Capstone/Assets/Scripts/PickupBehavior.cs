using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupBehavior : MonoBehaviour
{
    public enum PickUps{misslePickUp };
    public PickUps chosenPickUp;
    private Ability pickUp;
    // Start is called before the first frame update
    void Start()
    {
        
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        

        if (collision.gameObject.CompareTag("Vehicle"))
        {
            if(collision.gameObject.GetComponent<VehicleAbilityBehavior>().hasPickup() == false)
            {
                if (chosenPickUp == PickUps.misslePickUp)
                {
                    pickUp = Resources.Load<Ability>("MissileAbility");
                    collision.gameObject.GetComponent<VehicleAbilityBehavior>().assignPickup(pickUp);
                }
                
            }
        }
    }
}

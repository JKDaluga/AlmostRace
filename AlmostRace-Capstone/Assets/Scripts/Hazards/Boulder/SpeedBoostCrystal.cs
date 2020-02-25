using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedBoostCrystal : MonoBehaviour
{
    public float speedBoostPercentage;
    public bool isActive = false;

    public void ActivateCrystal()
    {
        isActive = true;
    }

    public void OnTriggerEnter(Collider other)
    {
        if(isActive)
        {
            if(other.gameObject.GetComponent<RaycastCar>() != null)
            {
                other.gameObject.GetComponent<RaycastCar>().setBoostPadSpeed(speedBoostPercentage);
            }
        }
    }

}

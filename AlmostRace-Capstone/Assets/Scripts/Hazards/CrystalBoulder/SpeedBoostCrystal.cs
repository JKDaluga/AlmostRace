using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedBoostCrystal : MonoBehaviour
{
    public float speedBoostPercentage;
    public float boostTime = 3f;
    public bool isActive = false;
    private List<RaycastCar> _boostedCars;
    private RaycastCar carToAdd;

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
                carToAdd = other.gameObject.GetComponent<RaycastCar>();
                if (_boostedCars.Contains(carToAdd))
                _boostedCars.Add(carToAdd);
                other.gameObject.GetComponent<RaycastCar>().setBoostPadSpeed(speedBoostPercentage);
            }
        }
       // StartCoroutine(ResetBoost());
    }

   // public IEnumerator ResetBoost()
   // {
        //other.gameObject.GetComponent<RaycastCar>().res
    //}
}

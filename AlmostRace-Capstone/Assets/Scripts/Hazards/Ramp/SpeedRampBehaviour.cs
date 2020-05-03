using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedRampBehaviour : MonoBehaviour
{
    public float speedBoostPercentage;
    public float boostTime = 2f;
    public bool isActive = false;
    private List<RaycastCar> _boostedCars = new List<RaycastCar>();
    private RaycastCar _carToAdd;
    public List<ParticleSystem> vfxToActivate = new List<ParticleSystem>();
    // Start is called before the first frame update
    public void OnTriggerEnter(Collider other)
    {
     
            if (other.gameObject.GetComponent<RaycastCar>() != null)
            {
            foreach (ParticleSystem vfx in vfxToActivate)
            {
                vfx.Play();
            }

            _carToAdd = other.gameObject.GetComponent<RaycastCar>();
                if (!_boostedCars.Contains(_carToAdd))
                {
                    _boostedCars.Add(_carToAdd);
                    other.gameObject.GetComponent<RaycastCar>().SetBoostPadSpeed(speedBoostPercentage / 100);
                       StartCoroutine(ResetBoost(boostTime, _carToAdd));
                }

            }
        

    }

    public IEnumerator ResetBoost(float timeToReset, RaycastCar carToReset)
    {
     
        yield return new WaitForSeconds(timeToReset);

        carToReset.gameObject.GetComponent<RaycastCar>().ResetBoostPadSpeed();
        _boostedCars.Remove(carToReset);
    }
}

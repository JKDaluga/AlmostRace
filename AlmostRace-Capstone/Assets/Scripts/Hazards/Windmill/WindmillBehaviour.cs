using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Eddie and Greg

public class WindmillBehaviour : MonoBehaviour
{

    public GameObject linkedMill;
    [Header("Laser Variables")]
    public GameObject[] linkedLaser;
    public GameObject interactingPlayer;
    public float laserSpinSpeed;
    public float laserSpeedUpAmount;
    public float laserDamageAmount, laserDmgRate;

    [Header("ShieldVariables")]
    public float extraShieldAmount;
    public float shieldDuration = 3f;
    private List<CarHealthBehavior> _shieldedCars = new List<CarHealthBehavior>();
    private CarHealthBehavior _carToAdd;

    // Start is called before the first frame update
    void Start()
    {
        linkedMill.GetComponent<WindmillSpeed>().UpdateBaseSpeed(laserSpinSpeed);
        for (int i = 0; i < linkedLaser.Length; i++)
        {
            linkedLaser[i].GetComponent<WindmillLaserBehaviour>().UpdateLaserDamage(laserDamageAmount);
            linkedLaser[i].GetComponent<WindmillLaserBehaviour>().UpdateDamageRate(laserDmgRate);
        }
    }


    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.GetComponent<CarHealthBehavior>() != null)
        {
            interactingPlayer = collision.gameObject;
            _carToAdd = collision.gameObject.GetComponent<CarHealthBehavior>();
            if (!_shieldedCars.Contains(_carToAdd))
            {
                _shieldedCars.Add(_carToAdd);
                _carToAdd.AddExtraShields(extraShieldAmount);
                StartCoroutine(ResetShield(shieldDuration, _carToAdd));
            }

            linkedMill.GetComponent<WindmillSpeed>().UpdateBaseSpeed(laserSpeedUpAmount);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<CarHealthBehavior>() != null)
        {
            Invoke("slowDown", 6.0f);
        }
    }

    void slowDown()
    {
        linkedMill.GetComponent<WindmillSpeed>().UpdateBaseSpeed(laserSpinSpeed);
    }

    public IEnumerator ResetShield(float timeToReset, CarHealthBehavior carToReset)
    {
        yield return new WaitForSeconds(timeToReset);
        carToReset.RemoveExtraShields(extraShieldAmount);
        _shieldedCars.Remove(carToReset);
       
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindmillBehaviour : MonoBehaviour
{

    public GameObject linkedMill;
    public GameObject[] linkedLaser;
    public GameObject interactingPlayer;
    public float laserSpinSpeed;
    public float speedUpAmount;
    public float extraShieldAmount;
    public float laserDamageAmount, laserDmgRate;

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
            interactingPlayer.GetComponent<CarHealthBehavior>().AddExtraHealth(extraShieldAmount);

            linkedMill.GetComponent<WindmillSpeed>().UpdateBaseSpeed(speedUpAmount);
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
}

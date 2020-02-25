using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindmillBehaviour : MonoBehaviour
{

    public GameObject linkedLaser;
    public GameObject interactingPlayer;
    public float laserSpeed;
    public float speedUpAmount;
    public float extraShieldAmount;

    // Start is called before the first frame update
    void Start()
    {
        linkedLaser.GetComponent<WindmillLaserBehaviour>().UpdateBaseSpeed(laserSpeed);
    }


    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.GetComponent<CarHealthBehavior>() != null)
        {
            interactingPlayer = collision.gameObject;
            interactingPlayer.GetComponent<CarHealthBehavior>().AddExtraHealth(extraShieldAmount);

            linkedLaser.GetComponent<WindmillLaserBehaviour>().UpdateBaseSpeed(speedUpAmount);
        }
    }

}

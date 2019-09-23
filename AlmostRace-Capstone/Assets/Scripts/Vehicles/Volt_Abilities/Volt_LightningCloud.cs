using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Edouard Borissov
 * This code handles the behavior of the Lightning Clouds spawned by the exploding Volt Missiles.
 * When a racer drives through a cloud, their engines stall.
 * 
 */

public class Volt_LightningCloud : MonoBehaviour
{
    private float _growthRate;
    private float _growthAmount;
    private float _cloudDuration;
    private float _cloudMaxSize;
    private GameObject _immunePlayer;


    void Start()
    {
        InvokeRepeating("Grow", 0, _growthRate);
        Destroy(gameObject, _cloudDuration);
    }

    public void Grow()
    {
        if(transform.localScale.x < _cloudMaxSize) //Checks if the cloud has reached max size.
        {
            transform.localScale += new Vector3(_growthAmount, _growthAmount, _growthAmount);
        }
        else
        {
            CancelInvoke("Grow"); //Stops the cloud from growing if it is max size.
        }
    }

    public void SetImmunePlayer(GameObject newImmunePlayer)
    {
        _immunePlayer = newImmunePlayer;
    }

    public void SetCloudInfo(float cloudDuration, float growthRate, float growthAmount, float cloudMaxSize)
    {
        _cloudDuration = cloudDuration;
        _growthRate = growthRate;
        _growthAmount = growthAmount;
        _cloudMaxSize = cloudMaxSize;

    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject != _immunePlayer)
        {
            if (collision.GetComponent<CarHeatManager>() != null)
            {
                collision.GetComponent<CarHeatManager>().heatCurrent = collision.GetComponent<CarHeatManager>().heatExplodeLimit;
                //CURRENTLY DEDICATED CODE FOR STALLING ENGINE DOESN'T EXIST. MAKE THIS CODE AND REPLACE THE LINE ABOVE!!!
            }
        }
        else if (collision.gameObject == _immunePlayer)
        {
            Debug.Log("Player was detected by lightning cloud");
        }
    }
}

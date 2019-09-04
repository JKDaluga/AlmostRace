using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LavaDropBehavior : MonoBehaviour
{
    public GameObject immunePlayer;
    public float shrinkRate = 0.1f;
    public float shrinkAmount = 0.1f;
    public float lavaDamage = 20f;


    void Start()
    {
        InvokeRepeating("Shrink", 0, shrinkRate);
    }


    public void Shrink()
    {
        transform.localScale -= new Vector3(shrinkAmount, shrinkAmount, shrinkAmount);
        if(transform.localScale.x <= .5)
        {
            Destroy(gameObject);
        }
    }

    public void OnTriggerEnter(Collider other)
    {    
        if(other.GetComponent<CarHeatManager>() != null)
        {
            if (other.gameObject != immunePlayer)
            {
                other.GetComponent<CarHeatManager>().heatCurrent += lavaDamage;
            }               
        }
    }
}

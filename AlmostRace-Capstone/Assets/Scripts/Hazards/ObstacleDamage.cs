using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    Creator and developer of script: Eddie Borrisov
    Purpose: Obstacles damage the players on impact
*/
public class ObstacleDamage : MonoBehaviour
{
    public float damage = 10;
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Vehicle"))
        {
            if (other.gameObject.GetComponent<CarHeatManager>() != null)
            {
                other.gameObject.GetComponent<CarHeatManager>().healthCurrent
                = other.gameObject.GetComponent<CarHeatManager>().healthCurrent + damage;
            }
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
    Creator and developer of script: Eddie Borrisov
    Purpose: KILLS PLAYERS THAT STEP OUT OF LINE
*/
public class KillboxBehaviour : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<CarHeatManager>() != null)
        {//if other is a car
            other.gameObject.GetComponent<CarHeatManager>().Kill();
        }
    }
}

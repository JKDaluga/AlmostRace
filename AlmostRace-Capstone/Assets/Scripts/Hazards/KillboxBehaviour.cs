using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillboxBehaviour : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<CarHeatManager>() != null)
        {//if other is a car
            other.gameObject.GetComponent<CarHeatManager>().AddHeat(other.gameObject.GetComponent<CarHeatManager>().healthMax);
        }
    }
}

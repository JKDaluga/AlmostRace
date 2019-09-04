using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleDamage : MonoBehaviour
{
    public float damage = 10;
    // Start is called before the first frame update
    void Start()
    {
        
    }


    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Vehicle"))
        {
            if (other.gameObject.GetComponent<CarHeatManager>() != null)
            {
                other.gameObject.GetComponent<CarHeatManager>().heatCurrent
                = other.gameObject.GetComponent<CarHeatManager>().heatCurrent + damage;
            }
        }
    }

}

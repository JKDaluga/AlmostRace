using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lux_EnergyBladeCenter : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            Destroy(gameObject.transform.parent.gameObject);
        }
    }
}

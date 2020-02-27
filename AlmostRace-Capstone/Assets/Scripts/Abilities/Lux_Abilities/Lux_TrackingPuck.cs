using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lux_TrackingPuck : Projectile
{
    // Start is called before the first frame update
    void Start()
    {

        GiveSpeed();      
    }

    private void OnTriggerEnter(Collider other)
    {
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionEffect : MonoBehaviour
{

    public Transform car;

    public void respawnEffect() { 
            Instantiate(Resources.Load("explosion"), car.position, car.rotation);
    }
    
}

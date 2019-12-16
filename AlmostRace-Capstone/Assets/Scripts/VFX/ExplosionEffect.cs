using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    Author: Leonardo Caballero
    Purpose: used for asyncronous loading of any scene
    CURRENTLY UNUSED
*/
public class ExplosionEffect : MonoBehaviour
{

    public Transform car;

    public void respawnEffect() { 
            Instantiate(Resources.Load("explosion"), car.position, car.rotation);
    }
    
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 The backend for hotspot checkpoint objects. A hotspot bot player will need to periodically
 reach these, or else they get penalized.

 This script handles resetting the Hotspot Player's checkpoint timer,
 as well as activated the next checkpoints that need to be reached.
 Currently, these "next" checkpoints will be assigned by hand by the map designers.
     */

public class Checkpoint_Hotspot : MonoBehaviour
{
    public List<GameObject> nextCheckpoints;
    private float _checkpointHype = 5; // private so that we don't have to manually set it in each object.

    private void OnTriggerEnter(Collider other)
    {
        /*
         * if(other.GetComponent<Jake's HotspotBot Script>() != null && other.GetComponent<Jake's HotspotBot Script>())
         * {
         *      if (other.GetComponent<Jake's HotspotBot Script>().hasHotspotBot == true)
         *      {
         *          other.GetComponent<Jake's HotspotBot Script>().ResetTimer();
         *          other.GetComponent<Jake's HotspotBot Script>().VentHeat();
         *          other.GetComponent<Jake's HotspotBot Script>().AddHype();
         *          foreach(GameObject obj in nextCheckpoints)
         *          {
         *              obj.SetActive(true);
         *          }
         *          gameObject.SetActive(false);  
         *            
         *      }
         * }

                */
    }
}

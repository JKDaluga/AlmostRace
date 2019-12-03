using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Robyn Riley 11/26/19
 * An assistant class for Designers to edit that controls the parameters of the aiming assist
 */

public class AimAssistant : MonoBehaviour
{

    public float assistWidth;
    public float assistLength;
    public LayerMask shootable;
    

    public GameObject aimCircle;

    public GameObject self;

    public GameObject nearest;

    RaycastHit hit;
    
    private void FixedUpdate()
    {

        //Checks if the collider object is the right size to allow for easy scaling at runtime, this can be shifted to start when the final values are done
        if (aimCircle != null)
        {

            if (aimCircle.transform.localScale.y != assistLength)
            {
                aimCircle.transform.localScale = new Vector3(aimCircle.transform.localScale.x, assistLength, aimCircle.transform.localScale.z);
                aimCircle.transform.localPosition = new Vector3(0, aimCircle.transform.localPosition.y, assistLength+1);
            }

            if (aimCircle.transform.localScale.x != assistWidth)
            {
                aimCircle.transform.localScale = new Vector3(assistWidth, aimCircle.transform.localScale.y, aimCircle.transform.localScale.z);
            }
        }
    }
}

﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class AdjustCenterOfMass : MonoBehaviour
{

    //How far the center of mass should be moved from its default
    //position
    [SerializeField] Vector3 centerOfMass = Vector3.zero;

    Vector3 downForce;

    private void Start()
    {
        //Override the center of mass, to enhance stability
        GetComponent<Rigidbody>().centerOfMass += centerOfMass;
        downForce = Vector3.down;
    }
    
    //Called by the editor to show "gizmos" in the Scene view. Used to
    //help visualize the overriden center of mass.
    private void OnDrawGizmosSelected()
    {
        //Draw a green sphere where the updated center of mass will be.
        Gizmos.color = Color.green;

        var currentCenterOfMass = this.GetComponent<Rigidbody>().worldCenterOfMass;

        Gizmos.DrawSphere(currentCenterOfMass + centerOfMass, 0.125f);

    }
}
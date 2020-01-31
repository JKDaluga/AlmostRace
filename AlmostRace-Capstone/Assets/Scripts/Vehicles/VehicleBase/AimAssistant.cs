using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Robyn Riley 11/26/19
 * An assistant class for Designers to edit that controls the parameters of the aiming assist
 */

public class AimAssistant : MonoBehaviour
{
    public LayerMask shootable, ground;

    public float visibleDistance;

    public GameObject aimObject, aimPos, aimCircle;


    [HideInInspector] public bool aimOn;

    public GameObject self;

    public GameObject target;

    RaycastHit hit;


    private void Start()
    {
        aimOn = true;
        aimCircle.GetComponent<AimCollider>().layer = ground;
        aimCircle.GetComponent<AimCollider>().maxDist = visibleDistance;
    }
    private void FixedUpdate()
    {


        if (target != null && aimOn)
        {
            aimPos.transform.position = GetComponent<AimAssistant>().target.GetComponent<Collider>().bounds.center;
        }
        else
        {
            aimPos.transform.localPosition = aimObject.transform.localPosition;
        }



        if (Input.GetButtonDown(GetComponent<VehicleInput>().rightStickButton))
        {
            aimOn = !aimOn;
        }
    }
}

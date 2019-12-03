using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimAssistant : MonoBehaviour
{

    public float assistRadius;
    public float assistLength;
    public LayerMask shootable;

    private Vector3 castEnd;

    public GameObject self;

    public GameObject nearest;

    RaycastHit hit;
    

    private void Start()
    {
        castEnd = (GetComponent<SphereCarController>().aimObject.transform.localPosition - transform.position) * assistLength;
    }

    private void FixedUpdate()
    {


        if (Physics.CapsuleCast(transform.position, castEnd, assistRadius, transform.parent.GetComponentInChildren<Camera>().gameObject.transform.forward, out hit, assistLength, shootable))
        {
            nearest = hit.collider.gameObject;

        }
        else
        {
            nearest = null;
        }


    }
}

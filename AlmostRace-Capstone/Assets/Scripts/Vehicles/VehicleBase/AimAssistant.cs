using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimAssistant : MonoBehaviour
{

    public float assistWidth;
    public float assistLength;
    public LayerMask shootable;

    private Vector3 castEnd;

    public GameObject aimCircle;

    public GameObject self;

    public GameObject nearest;

    RaycastHit hit;
    

    private void Start()
    {
        castEnd = (GetComponent<SphereCarController>().aimObject.transform.localPosition - transform.position) * assistLength;
    }

    private void FixedUpdate()
    {
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuspensionPoint : MonoBehaviour
{
    //checks how "compressed" each point is, 0 for fully extended, 1 for fully compressed
    private float compression;

    //value for how long each suspension point should be
    public float suspensionLength;

    //how much force is applied to keep vehicle lifted
    public float suspensionForce;

    private void FixedUpdate()
    {
        //draws a raycast downward to check for collision with objects
        Ray ray = new Ray(transform.position, -transform.up);
        RaycastHit hit;

        Debug.DrawRay(transform.position, -transform.up, Color.red);
        

        
        if(Physics.Raycast(ray, out hit, suspensionLength))
        {
            compression = (suspensionLength - hit.distance) / suspensionLength;
        }
        else
        {
            compression = 0;
        }

        lift();
    }


    //applies an unward force relative to the amount of compression
    public void lift()
    {
        
        if(compression > 0)
        {
            Vector3 force = Vector3.up * compression * suspensionForce;
            GetComponentInParent<Rigidbody>().AddForceAtPosition(force, transform.position, ForceMode.Acceleration);
        }
    }


}

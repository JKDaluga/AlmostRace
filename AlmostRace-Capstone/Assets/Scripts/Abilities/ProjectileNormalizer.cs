using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    Creator and developer of script: Jason Daluga
    Purpose: A script used to help projectiles ride slopes
*/
public class ProjectileNormalizer : MonoBehaviour
{
    private Rigidbody rb;
    public LayerMask layerMask;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        RaycastHit hitNear;
        Physics.Raycast(transform.position, Vector3.down, out hitNear, 5.0f, layerMask);

        if (hitNear.collider != null)
        {
            transform.up = hitNear.normal;
            transform.position = transform.position + ((4f - hitNear.distance) * transform.up);
            rb.velocity = Vector3.ProjectOnPlane(rb.velocity, transform.up).normalized * rb.velocity.magnitude;
            if(rb.velocity.magnitude != 0) transform.rotation = Quaternion.LookRotation(rb.velocity, transform.up);
        }

    }
}

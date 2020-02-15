using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    Creator and developer of script: Jason Daluga
    Purpose: A script used to help projectiles ride slopes
*/
public class ProjectileNormalizer : MonoBehaviour
{

    public LayerMask layerMask;

    // Update is called once per frame
    void Update()
    {
        RaycastHit hitNear;
        Physics.Raycast(transform.position, Vector3.down, out hitNear, 5.0f, layerMask);

        if (hitNear.collider != null)
        {
            transform.up = hitNear.normal;
            transform.position = transform.position + ((5f - hitNear.distance) * transform.up);
        }

    }
}

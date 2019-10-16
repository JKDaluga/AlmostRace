using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileNormalizer : MonoBehaviour
{

    public LayerMask layerMask;

    // Update is called once per frame
    void Update()
    {
        RaycastHit hitNear;
        Physics.Raycast(transform.position + (transform.forward * 1f), Vector3.down, out hitNear, 5.0f, layerMask);

        if (hitNear.collider != null)
        {
            float y = transform.eulerAngles.y;
            transform.up = hitNear.normal;//Vector3.Lerp(transform.up, hitNear.normal, Time.deltaTime * 8.0f);
            transform.Rotate(0, y, 0);
        }
    }
}

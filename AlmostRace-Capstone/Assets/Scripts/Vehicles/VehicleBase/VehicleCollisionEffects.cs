using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleCollisionEffects : MonoBehaviour
{

    public float velocityRequirement;
    public GameObject sparks;
    public Rigidbody colliderRigidbody;

    private void OnCollisionEnter(Collision collision)
    {
        foreach (ContactPoint contact in collision.contacts)
        {
            Instantiate(sparks, contact.point, Quaternion.identity);
        }        
    }

    private void OnCollisionStay(Collision collision)
    {
        foreach (ContactPoint contact in collision.contacts)
        {
            if (colliderRigidbody.velocity.magnitude > velocityRequirement)
            {
                Instantiate(sparks, contact.point, Quaternion.identity);
            }
        }
    }
}

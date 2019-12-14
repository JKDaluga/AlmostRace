using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class VehicleCollisionEffects : MonoBehaviour
{

    public float velocityRequirement;
    public GameObject sparks;
    public Rigidbody colliderRigidbody;

    private void OnCollisionEnter(Collision collision)
    {
        GetComponent<CinemachineImpulseSource>().m_ImpulseDefinition.m_AmplitudeGain = 1;
        GetComponent<CinemachineImpulseSource>().m_ImpulseDefinition.m_FrequencyGain = 1;

        GetComponent<CinemachineImpulseSource>().GenerateImpulse();
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

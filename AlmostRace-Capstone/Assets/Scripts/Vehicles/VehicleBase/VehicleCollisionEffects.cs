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
        CreateSparks(collision);
    }

    private void OnCollisionStay(Collision collision)
    {
        if (colliderRigidbody.velocity.magnitude > velocityRequirement)
        {
            CreateSparks(collision);
        }
    }

    public void CreateSparks(Collision givenCollision)
    {
        foreach (ContactPoint contact in givenCollision.contacts)
        {
            Instantiate(sparks, contact.point, Quaternion.identity);
        }  
    }
}

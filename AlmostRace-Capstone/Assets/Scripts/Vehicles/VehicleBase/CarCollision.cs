using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*Last Edited by Robyn, 10/30, Added hype generation to script*/

public class CarCollision : MonoBehaviour
{
    public GameObject frontCollisionParticles;
    public SphereCarController car;
    public VehicleCollisionEffects collisionEffectsScript;
    private float _invulTime = 0f;
    public float weight;

    public float hypeToAdd = 5;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        _invulTime -= Time.deltaTime;
    }

    private void OnCollisionEnter(Collision collision)
    {
        CarCollision other = collision.gameObject.GetComponent<CarCollision>();
        if (other != null && _invulTime <= 0f)
        {
            _invulTime = .5f;
            Rigidbody rb = GetComponent<Rigidbody>();
            Rigidbody other_rb = other.GetComponent<Rigidbody>();
            SphereCarController otherCar = other.car.GetComponent<SphereCarController>();
            CarHeatManager otherCarHeat = other.car.GetComponent<CarHeatManager>();
            Vector3 relativeVelocity = rb.velocity - other_rb.velocity;
            Vector3 relativePosition = rb.position - other_rb.position;
            collisionEffectsScript.CreateSparks(collision);
            AudioManager.instance.Play("General collision");

            //calculating the angle in radians then converting it to degrees
            float angleBetween = Mathf.Acos(Vector3.Dot(relativeVelocity.normalized, relativePosition.normalized))* 180 / Mathf.PI;
            
            // We only want the player instigating the collision to damage the other so this angle helps determing who the instigating player is
            float instigatingPlayerAngle = Mathf.Acos(Vector3.Dot(rb.velocity.normalized, -relativePosition.normalized)) * 180 / Mathf.PI;

            frontCollisionParticles.GetComponent<ParticleSystem>().Play();

            // If the angleBetween < 90 then the collision exists
            // If instigatingPlayerAngle < 90 then that player is moving in the direction of the other player and should assign damage
            if (angleBetween < 90 & instigatingPlayerAngle < 90)
            {
                float percentDamage = 0f;
                if (rb.velocity.magnitude > 10f)
                {
                    percentDamage = Vector3.Project(relativePosition.normalized, rb.velocity.normalized).magnitude;
                }
                if (percentDamage > 0)
                {
                    float damage = percentDamage * weight * relativeVelocity.magnitude;
                    otherCarHeat.healthCurrent -= damage;
                    car.GetComponent<VehicleHypeBehavior>().AddHype(hypeToAdd, "Crash!");
                }
            }
        }
    }
}

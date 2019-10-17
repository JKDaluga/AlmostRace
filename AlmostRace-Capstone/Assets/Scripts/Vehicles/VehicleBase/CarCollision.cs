using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarCollision : MonoBehaviour
{

    public SphereCarController car;
    private float _invulTime = 0f;
    public float weight;
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
            _invulTime = .2f;
            Rigidbody rb = GetComponent<Rigidbody>();
            Rigidbody other_rb = other.GetComponent<Rigidbody>();
            SphereCarController otherCar = other.car.GetComponent<SphereCarController>();
            CarHeatManager otherCarHeat = other.car.GetComponent<CarHeatManager>();
            Vector3 relativeVelocity = other_rb.velocity - rb.velocity;
            Vector3 relativePosition = other_rb.position - rb.position;
            float f = Vector3.Angle(relativePosition, rb.velocity);
            if (f < 110)
            {
                float percentDamage = Vector3.Project(relativePosition.normalized, rb.velocity.normalized).magnitude;
                float damage = percentDamage * weight * relativeVelocity.magnitude;
                print("YA BOI " + damage);
                otherCarHeat.heatCurrent += damage;
            }
        }
    }
}

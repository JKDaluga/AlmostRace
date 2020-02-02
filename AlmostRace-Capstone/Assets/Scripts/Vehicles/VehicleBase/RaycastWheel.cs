using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastWheel : MonoBehaviour
{
    public float maxSuspension = 0.2f;
    public LayerMask layerMask;
    private float spring = 100.0f;
    private float damper = 50.0f;

    public Rigidbody parent;
    private bool grounded = false;
    private float equilibrium;
    private RaycastCar car;

    private void Awake()
    {
        car = parent.GetComponent<RaycastCar>();
    }

    public bool IsGrounded
    {
        get
        {
            return grounded;
        }
    }


    void FixedUpdate()
    {
        GetGround();
    }

    void GetGround()
    {
        grounded = false;
        Vector3 downwards = transform.TransformDirection(-Vector3.up);
        RaycastHit hit;

        // down = local downwards direction
        Vector3 down = transform.TransformDirection(Vector3.down);

        if (Physics.Raycast(transform.position, downwards, out hit, maxSuspension, layerMask))
        {

            grounded = true;
            // the velocity at point of contact
            Vector3 velocityAtTouch = parent.GetPointVelocity(hit.point);

            // calculate spring compression
            // difference in positions divided by total suspension range
            float compression = Mathf.Pow(hit.distance / maxSuspension, 2);

            compression = -compression + 1;

            if (hit.distance / maxSuspension < equilibrium - .05f)
            {
                compression += .3f;
            }

            // final force
            Vector3 force = -downwards * compression * spring;
            // velocity at point of contact transformed into local space
            
            Vector3 t = transform.InverseTransformDirection(velocityAtTouch);

            // local x and z directions = 0
            t = Vector3.Project(velocityAtTouch, downwards);
            
            // back to world space * -damping
            Vector3 damping = t * -damper;
            Vector3 finalForce = force + damping;

            parent.AddForceAtPosition(finalForce, hit.point);
        }

        float speed = parent.velocity.magnitude;
    }

    public void setSpring(float springForce)
    {
        spring = springForce;
        equilibrium = (car.gravity * car.getCarMass()) / spring;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0, 0, 1, 1);
        Vector3 direction = transform.TransformDirection(-Vector3.up) * (this.maxSuspension);
        Gizmos.DrawRay(new Vector3(transform.position.x, transform.position.y, transform.position.z), direction);
    }
}

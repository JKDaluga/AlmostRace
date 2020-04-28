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
    RaycastHit hit;
    Vector3 downwards;
    Vector3 down;
    Vector3 velocityAtTouch;
    float compression;
    Vector3 force;
    Vector3 t;
    Vector3 damping;
    Vector3 finalForce;
    float speed;

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
        downwards = transform.TransformDirection(-Vector3.up);
       

        // down = local downwards direction
        down = transform.TransformDirection(Vector3.down);

        if (Physics.Raycast(transform.position, downwards, out hit, maxSuspension, layerMask))
        {

            grounded = true;
            // the velocity at point of contact
            velocityAtTouch = parent.GetPointVelocity(hit.point);

            // calculate spring compression
            // difference in positions divided by total suspension range
           compression = Mathf.Pow(hit.distance / maxSuspension, 2);

            compression = -compression + 1;

            if (hit.distance / maxSuspension < equilibrium - .05f)
            {
                compression += .3f;
            }

            // final force
            force = -downwards * compression * spring;
            // velocity at point of contact transformed into local space
            
            t = transform.InverseTransformDirection(velocityAtTouch);

            // local x and z directions = 0
            t = Vector3.Project(velocityAtTouch, downwards);
            
            // back to world space * -damping
            damping = t * -damper;
            finalForce = force + damping;

            parent.AddForceAtPosition(finalForce, hit.point);

            //corkscrew workaround
            if(hit.collider.gameObject.tag.Equals("Corkscrew") && car.gravity < 500)
            {
                car.gravity += 10;
                if (!car.onCorkscrew)
                {
                    car.onCorkscrew = true;
                }
            }
        }

        speed = parent.velocity.magnitude;
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

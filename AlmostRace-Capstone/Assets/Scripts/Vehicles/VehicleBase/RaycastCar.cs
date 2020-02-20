using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastCar : MonoBehaviour
{
    // car physics calculations/input stuff
    private Vector3 accel;
    [Header("Input Variables")]
    public float throttle;
    public float reverse;
    public float horizontal;
    public bool drift;

    [Header("Car Testing Variables")]
    public float currentSpeed;
    public float currentTurnSpeed;
    public float actualGrip;
    private float deadZone = .1f;
    private Vector3 myRight;
    private Vector3 vel;
    private Vector3 flatVel;
    private Vector3 relativeVel;
    private Vector3 dir;
    private Vector3 flatDir;
    private Vector3 carUp;
    private Transform carTransform;
    private Rigidbody carRigidbody;
    private Vector3 engineForce;
    private Vector3 turnVec;
    private Vector3 imp;
    private float rev;
    private float actualTurn;
    private float carMass;
    private float boostSpeed = 0f;
    private float boostPadSpeed = 0f;

    [Header("Car UI")]
    public RectTransform UIPanel;
    public RectTransform secondaryUIPanel;

    // the physical transforms for the car's wheels
    [Header("Wheels")]
    public Transform frontLeftWheel;
    public Transform frontRightWheel;
    public Transform rearLeftWheel;
    public Transform rearRightWheel;

    public RaycastWheel frontLeftRayCast;
    public RaycastWheel frontRightRayCast;
    public RaycastWheel rearLeftRayCast;
    public RaycastWheel rearRightRayCast;

    // car physics adjustments
    [Header("Car Adjustable Variables")]
    public float gravity = 200f;
    public float power = 300;
    public float maxSpeed = 50;
    public float maxTurnSpeed = 10;
    public float carGrip = 70;
    public float turnSpeed = 3.0f;  //keep this value somewhere between 2.5f and 6.0f
    public float driftStrength = 1.5f; //this number should be greater than 1f
    public float springFallDisengangeSpeed = 50f;
    public float spring = 4000f;

    public float deceleration = 5f;
    public float turnBackMultiplier  = 1.5f;


    private float slideSpeed;

    private Vector3 carRight;
    private Vector3 carFwd;
    private Vector3 tempVEC;
    private Vector3 gravityDirection;

    private VehicleInput input;

    void Start()
    {
        Initialize();
    }

    void Initialize()
    {
        // Cache Vehicle Input
        input = GetComponent<VehicleInput>();
        // Cache a reference to our car's transform
        carTransform = transform;
        // cache the rigidbody for our car
        carRigidbody = GetComponent<Rigidbody>();
        // cache the mass of our vehicle
        carMass = GetComponent<Rigidbody>().mass;
        // call to set up our wheels array
        setUpWheels();
        // we set a COG here and lower the center of mass to a
        //negative value in Y axis to prevent car from flipping over
        carRigidbody.centerOfMass = new Vector3(0f, -1.0f, -0f);
        frontLeftRayCast = frontLeftWheel.GetComponentInParent<RaycastWheel>();
        frontRightRayCast = frontRightWheel.GetComponentInParent<RaycastWheel>();
        rearLeftRayCast = rearLeftWheel.GetComponentInParent<RaycastWheel>();
        rearRightRayCast = rearRightWheel.GetComponentInParent<RaycastWheel>();
        gravityDirection = -transform.up;
    }

    void Update()
    {
        // cache our vector up direction
        carUp = carTransform.up;
        // cache the Forward World Vector for our car
        carFwd = Vector3.forward;
        // cache the World Right Vector for our car
        carRight = Vector3.right;

        //call the function to see what input we are using and apply it
        checkInput();

        // call the function to start processing all vehicle physics
        carPhysicsUpdate();
    }

    void setUpWheels()
    {
        if ((null == frontLeftWheel || null == frontRightWheel || null == rearLeftWheel || null == rearRightWheel))
        {
            Debug.LogError("One or more of the wheel transforms have not been plugged in on the car");
            Debug.Break();
        }
    }

    void checkInput()
    {
        // If there is no vehicle input then this is an AI Carand the AI Car will set the inputs.
        if (input != null)
        {
            if (input.getStatus())
            {
                horizontal = Input.GetAxis(input.horizontal);
                throttle = Input.GetAxis(input.verticalForward);
                reverse = Input.GetAxis(input.verticalBackward);
                drift = Input.GetButton(input.brake);
            }
            else
            {
                horizontal = 0f;
                throttle = 0f;
                reverse = 0f;
                drift = false;
            }
        }
    }

    void carPhysicsUpdate()
    {
        //grab all the physics info we need to calc everything
        myRight = carTransform.right;

        // find our velocity
        Vector3 vel = carRigidbody.velocity;

        Vector3 tempVEC = Vector3.ProjectOnPlane(vel, carUp); //new Vector3(velo.x, 0, velo.z);

        // figure out our velocity without y movement - our flat velocity
        flatVel = tempVEC;

        // find out which direction we are moving in
        dir = transform.TransformDirection(carFwd);

        tempVEC = Vector3.ProjectOnPlane(dir, carUp);//new Vector3(dir.x, 0, dir.z);

        // calculate our direction, removing y movement - our flat direction
        flatDir = Vector3.Normalize(tempVEC);
        
        // calculate relative velocity
        relativeVel = carTransform.InverseTransformDirection(vel);
        
        if(relativeVel.y < -springFallDisengangeSpeed) setSpringForce(spring/2f);
        else setSpringForce(spring);

        // calculate how much we are sliding (find out movement along our x axis)
        slideSpeed = Vector3.Dot(myRight, flatVel);

        // calculate current speed (the magnitude of the flat velocity)
        currentSpeed = flatVel.magnitude;
        currentTurnSpeed = Mathf.Abs(carRigidbody.angularVelocity.y);

        // calculate engine force with our flat direction vector and acceleration
        engineForce = (flatDir * (power * (throttle - reverse)) * carMass);

        // do turning
        actualTurn = horizontal;

        // calculate torque for applying to our rigidbody
        turnVec = (((carUp * turnSpeed) * actualTurn) * carMass) * 800;

        // calculate impulses to simulate grip by taking our right vector, reversing the slidespeed and
        // multiplying that by our mass, to give us a completely 'corrected' force that would completely
        // stop sliding. we then multiply that by our grip amount (which is, technically, a slide amount) which
        // reduces the corrected force so that it only helps to reduce sliding rather than completely
        // stop it 

        actualGrip = Mathf.Lerp(100, carGrip, currentSpeed * 0.02f);
        imp = myRight * (-slideSpeed * carMass * actualGrip);

    }

    void FixedUpdate()
    {
        //gravity
        if(isCarGrounded())
        {
            carRigidbody.AddForce(-carUp.normalized * gravity * carRigidbody.mass);
            gravityDirection = -carUp;
        }
        else
        {
            carRigidbody.AddForce(gravityDirection.normalized * gravity * carRigidbody.mass);
        }
        

        if (currentSpeed < maxSpeed + boostSpeed + boostPadSpeed)
        {
            // apply the engine force to the rigidbody
            carRigidbody.AddForce(engineForce * Time.deltaTime);
        }

        // turn car
        float tempMaxTurnSpeed = maxTurnSpeed;
        if(drift)
        {
            tempMaxTurnSpeed *= driftStrength;
        }

        if (currentTurnSpeed < tempMaxTurnSpeed || carRigidbody.angularVelocity.y * horizontal < 0f)
        {
            if(carRigidbody.angularVelocity.y * horizontal < 0f)
            {
                carRigidbody.AddTorque(turnBackMultiplier * turnVec * Time.deltaTime);
            }
            else
            {
                carRigidbody.AddTorque(turnVec * Time.deltaTime);
            }
        }

        if (Mathf.Abs(horizontal) >= deadZone)
        {
            carRigidbody.angularDrag = 0f;
        }
        else
        {
            carRigidbody.angularDrag = 10f;
        }

        if ((Mathf.Abs(throttle) >= deadZone && Mathf.Abs(reverse) <= deadZone || Mathf.Abs(reverse) >= deadZone && Mathf.Abs(throttle) <= deadZone) || !isCarGrounded())
        {
            carRigidbody.drag = 0f;
        }
        else
        {
            carRigidbody.drag = deceleration;
        }

        // apply forces to our rigidbody for grip
        carRigidbody.AddForce(imp * Time.deltaTime);
    }

    public float getCarMass()
    {
        return carMass;
    }

    public Vector3 GetRelativeVelocity()
    {
        return relativeVel;
    }
    
    public Vector3 GetFlatVelocity()
    {
        return flatVel;
    }

    
    //Car is grounded if atleast one of those wheels are grounded
    public bool isCarGrounded()
    {
        return frontLeftRayCast.IsGrounded || frontRightRayCast.IsGrounded || rearLeftRayCast.IsGrounded || rearRightRayCast.IsGrounded;
    }

    private void setSpringForce(float springForce)
    {
        frontLeftRayCast.setSpring(springForce);
        frontRightRayCast.setSpring(springForce);
        rearLeftRayCast.setSpring(springForce);
        rearRightRayCast.setSpring(springForce);
    }

    public void cheatPhysics()
    {
        if (currentSpeed > maxSpeed + boostSpeed + boostPadSpeed)
        {
            carRigidbody.velocity = maxSpeed * carRigidbody.velocity.normalized;
        }
    }

    public void setBoostSpeed(float percentage)
    {
        boostSpeed = maxSpeed * percentage;
    }

    public void setBoostPadSpeed(float percentage)
    {
        boostPadSpeed = maxSpeed * percentage;
    }
}

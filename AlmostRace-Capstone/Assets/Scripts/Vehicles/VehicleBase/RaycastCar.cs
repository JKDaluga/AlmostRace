using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastCar : MonoBehaviour
{
    // car physics calculations/input stuff
    private Vector3 accel;
    [Header("Car Testing Variables")]
    public float throttle;
    public float reverse;
    private float deadZone = .1f;
    private Vector3 myRight;
    private Vector3 velo;
    private Vector3 flatVelo;
    private Vector3 relativeVelocity;
    private Vector3 dir;
    private Vector3 flatDir;
    private Vector3 carUp;
    private Transform carTransform;
    private Rigidbody carRigidbody;
    private Vector3 engineForce;
    public float currentSpeed;
    public float currentTurnSpeed;

    private Vector3 turnVec;
    private Vector3 imp;
    private float rev;
    private float actualTurn;
    private float carMass;
    private Transform[] wheelTransform = new Transform[4]; //these are the transforms for our 4 wheels
    public float actualGrip;
    public float horizontal; //horizontal input control, either mobile control or keyboard

    // the physical transforms for the car's wheels
    [Header("Wheels")]
    public Transform frontLeftWheel;
    public Transform frontRightWheel;
    public Transform rearLeftWheel;
    public Transform rearRightWheel;

    public RaycastWheel frontLeftRayCast;
    public RaycastWheel frontRightRayCast;

    // car physics adjustments
    [Header("Car Adjustable Variables")]
    public float gravity = 200f;
    public float power = 300;
    public float maxSpeed = 50;
    public float maxTurnSpeed = 10;
    public float carGrip = 70;
    public float turnSpeed = 3.0f;  //keep this value somewhere between 2.5f and 6.0f
    public float driftStrength = 1.5f; //this number should be greater than 1f

    private float slideSpeed;

    private Vector3 carRight;
    private Vector3 carFwd;
    private Vector3 tempVEC;

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
        else
        {
            //set up the car's wheel transforms
            wheelTransform[0] = frontLeftWheel;
            wheelTransform[1] = rearLeftWheel;
            wheelTransform[2] = frontRightWheel;
            wheelTransform[3] = rearRightWheel;
        }
    }

    void checkInput()
    {
        //Use the Keyboard for all car input
        horizontal = Input.GetAxis(input.horizontal);
        throttle = Input.GetAxis(input.verticalForward);
        reverse = Input.GetAxis(input.verticalBackward);
    }

    void carPhysicsUpdate()
    {
        //grab all the physics info we need to calc everything
        myRight = carTransform.right;

        // find our velocity
        Vector3 velo = carRigidbody.velocity;

        Vector3 tempVEC = Vector3.ProjectOnPlane(velo, carUp); //new Vector3(velo.x, 0, velo.z);

        // figure out our velocity without y movement - our flat velocity
        flatVelo = tempVEC;

        // find out which direction we are moving in
        dir = transform.TransformDirection(carFwd);

        tempVEC = Vector3.ProjectOnPlane(dir, carUp);//new Vector3(dir.x, 0, dir.z);

        // calculate our direction, removing y movement - our flat direction
        flatDir = Vector3.Normalize(tempVEC);
        
        // calculate relative velocity
        relativeVelocity = carTransform.InverseTransformDirection(flatVelo);

        // calculate how much we are sliding (find out movement along our x axis)
        slideSpeed = Vector3.Dot(myRight, flatVelo);

        // calculate current speed (the magnitude of the flat velocity)
        currentSpeed = flatVelo.magnitude;
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
        carRigidbody.AddForce(-carUp.normalized * gravity * carRigidbody.mass);

        if (currentSpeed < maxSpeed)
        {
            // apply the engine force to the rigidbody
            carRigidbody.AddForce(engineForce * Time.deltaTime);
        }

        // turn car
        float tempMaxTurnSpeed = maxTurnSpeed;
        if(Input.GetButton(input.brake))
        {
            tempMaxTurnSpeed *= driftStrength;
        }

        if (currentTurnSpeed < tempMaxTurnSpeed || carRigidbody.angularVelocity.y * horizontal < 0f)
        {
            carRigidbody.AddTorque(turnVec * Time.deltaTime);
        }

        if (Mathf.Abs(horizontal) >= deadZone)
        {
            carRigidbody.angularDrag = 0f;
        }
        else
        {
            carRigidbody.angularDrag = 10f;
        }

        if (Mathf.Abs(throttle) >= deadZone && Mathf.Abs(reverse) <= deadZone || Mathf.Abs(reverse) >= deadZone && Mathf.Abs(throttle) <= deadZone)
        {
            carRigidbody.drag = 0f;
        }
        else
        {
            carRigidbody.drag = 5f;
        }

        // apply forces to our rigidbody for grip
        carRigidbody.AddForce(imp * Time.deltaTime);
    }

    public float getCarMass()
    {
        return carMass;
    }
}

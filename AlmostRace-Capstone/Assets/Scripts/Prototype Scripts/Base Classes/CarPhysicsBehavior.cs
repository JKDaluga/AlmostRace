using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CarPhysicsBehavior : MonoBehaviour
{
    private CarHeatManager carHeatInfo;
    public Image carSpeedUI;
    //Downward force applied to vehicle to keep it on the ground
    public float downForce = 100;

    //Call for the rigidbody of the car
    private Rigidbody carRB;

    //The transform position where driving and braking forces are applies
    public Transform drivePos;

    //forces applied by each action
    public float driveForce, brakeForce, turnForce;

    // Acceleration to be added when driving
    public float acceleration;
    public float deceleration;

    // The current speed of the vehicle
    [HideInInspector] public float currentDriveForce;

    float deadZone = .1f;

    public GameObject[] hoverPoints;

    public float groundedDrag = 3.0f;

    [Range(0, 1)]
    public float reverseSpeed = 0.25f;

    //the input fields for each action
    private float _driveInput, _brakeInput, _turnInput;

    //the grip value that controls how much the car slides when turning
    public float carGrip;

    //current speed
    private float _currSpeed;

    //checks how "compressed" each point is, 0 for fully extended, 1 for fully compressed
    private float _compression;

    private GameObject _suspensionPoint;

    //value for how long each suspension point should be
    public float suspensionLength = 3;

    //how much force is applied to keep vehicle lifted
    public float suspensionForce = 50;

    public bool grounded;

    public float maxSpeed = 500.0f;

    //The forward value without the upward component
    private Vector3 _flatFwd;

    //Input Axis for controller support
    public string horizontalAxis; //left stick
    public string verticalForwardAxis;//right trigger
    public string verticalBackwardAxis;//left trigger
    public float forwardInput;
    public float backwardInput;
    private VehicleInput _vehicleInput;

    private void Awake()
    {
        //stores the rigidbody value of the car
        carRB = GetComponent<Rigidbody>();
        _vehicleInput = GetComponent<VehicleInput>();
    }

    private void Start()
    {
        //lowers the center of mass of the vehicle to limit flipping
        carRB.centerOfMass = new Vector3(0, -1, 0);
        carHeatInfo = gameObject.GetComponent<CarHeatManager>();
    }

    private void FixedUpdate()
    {
        _turnInput = Input.GetAxis(_vehicleInput.horizontal);

        forwardInput = Input.GetAxis(_vehicleInput.verticalForward);
        backwardInput = Input.GetAxis(_vehicleInput.verticalBackward);

        //clamps braking and throttle inputs to needed values
        _driveInput = Mathf.Clamp(_driveInput, -reverseSpeed, 1);

        _brakeInput = Mathf.Clamp(_brakeInput, -1, 0);

        //Consolidated suspension system into this script. Draws a downward raycast at each point to check for collisions and applies an upward force if one is found.
        for (int i = 0; i < hoverPoints.Length; i++)
        {
            _suspensionPoint = hoverPoints[i];

            Ray ray = new Ray(_suspensionPoint.transform.position, -transform.up);
            RaycastHit hit;

            Debug.DrawRay(_suspensionPoint.transform.position, -transform.up, Color.red);


            //Checks if the car is touching the ground at all positions
            if (Physics.Raycast(ray, out hit, suspensionLength))
            {
                _compression = (suspensionLength - hit.distance) / suspensionLength;
                grounded = true;
            }
            else
            {
                _compression = 0;
                grounded = false;
            }

            if (_compression > 0)
            {
                Vector3 force = Vector3.up * _compression * suspensionForce;
                carRB.AddForceAtPosition(force, transform.position, ForceMode.Acceleration);
            }
        }

        //adjusts drag value based on if car is in the air or on the ground
        if (grounded)
        {
            carRB.drag = groundedDrag;
        }
        else
        {
            carRB.drag = groundedDrag;
        }



        carRB.AddForce(-Vector3.up * downForce, ForceMode.Acceleration);

        //calculates out the forward vector of the vehicle so it won't fly upward when throttle is applied
        Vector3 carFwd = transform.TransformDirection(Vector3.forward);
        Vector3 tempFwd = new Vector3(carFwd.x, 0, carFwd.z);

        _flatFwd = Vector3.Normalize(tempFwd);

        throttle();
        brake();
        turn();
        slideControl();


        //Checks if car is above the max speed and reduces it's velocity if so
        if (carRB.velocity.sqrMagnitude > (carRB.velocity.normalized * maxSpeed).sqrMagnitude)
        {
            carRB.velocity = carRB.velocity.normalized * maxSpeed;
        }
    }

    //applies forward force based on inputs
    public void throttle()
    {
        if (forwardInput > deadZone && (carHeatInfo.heatCurrent < carHeatInfo.heatStallLimit))
        {
            currentDriveForce += acceleration * Time.fixedDeltaTime;
            currentDriveForce = Mathf.Clamp (currentDriveForce, 0, driveForce);
        }
        else if (  (forwardInput < 0 && currentDriveForce > 0f) 
                || (backwardInput > 0)                           )
        {
            currentDriveForce -= (deceleration + 100) * Time.fixedDeltaTime;
            currentDriveForce = Mathf.Clamp (currentDriveForce, -200f, driveForce);
        }
        else if (forwardInput <= deadZone && forwardInput >= 0 || carHeatInfo.heatCurrent >= carHeatInfo.heatStallLimit)
        {
            if (currentDriveForce > 0) {
                currentDriveForce -= deceleration * Time.fixedDeltaTime;
            }
            currentDriveForce = Mathf.Clamp (currentDriveForce, 0, driveForce);
        }
        carRB.AddForce(_flatFwd * currentDriveForce); //used for W and S and arrow keys
    }

    //applies backward force based on inputs
    public void brake()
    {
        if (carRB.velocity.z > 0 && grounded)
        {
            carRB.AddForceAtPosition(_flatFwd * brakeForce * _brakeInput, drivePos.position);
        }
    }

    //applies a torque to rotate the vehicle the appropriate amount
    public void turn()
    {
        if (_turnInput != 0)
        {
            carRB.AddRelativeTorque(Vector3.up * _turnInput * turnForce);
        }
    }


    //applies an inverse sideways force to limit vehicle sliding when turning
    public void slideControl()
    {

        Vector3 tempVel = new Vector3(carRB.velocity.x, 0, carRB.velocity.z);
        Vector3 flatVel = Vector3.Normalize(tempVel);

        _currSpeed = flatVel.magnitude;

        float sliding = Vector3.Dot(transform.right, flatVel);

        float slideAmount = Mathf.Lerp(100, carGrip, _currSpeed * .02f);

        Vector3 slideControl = transform.right * (-sliding * slideAmount);
    }

}

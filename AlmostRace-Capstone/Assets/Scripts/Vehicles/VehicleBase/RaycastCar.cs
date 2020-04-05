using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class RaycastCar : MonoBehaviour
{
    // car physics calculations/input stuff
    private Vector3 accel;
    [Header("Input Variables")]
    public float throttle;
    public float reverse;
    public float horizontal;
    public bool drift;
    public PlayerUIManager playerUIManagerScript;

    [Header("Car Testing Variables")]
    public float currentSpeed;
    public float currentTurnSpeed;
    public float actualGrip;
    public Vector3 relativeAngularVel;
    public int playerID = 0;
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

    private bool resetBoostTrigger = false;
    private float resetBoostTime = 0f;
    private bool resetBoostPadTrigger = false;
    private float resetBoostPadTime = 0f;

    [Header("Car UI")]
    public RectTransform UIPanel;
    public RectTransform secondaryUIPanel;

    // the physical transforms for the car's wheels
    [Header("Wheels")]
    RaycastHit hit;
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
    public float maxTurnAngle = 10;
    public float carGrip = 70;
    public float turnSpeed = 3.0f;  //keep this value somewhere between 2.5f and 6.0f
    public float driftStrength = 1.5f; //this number should be greater than 1f
    public float springFallDisengangeSpeed = 50f;
    public float spring = 4000f;

    public float deceleration = 5f;
    public float turnBackMultiplier  = 1.5f;
    public float boostAccelerationMulti = .5f;


    private float slideSpeed;
    public bool isBoosting;

    [Header("FOV Variables")]
    public CinemachineVirtualCamera cineCamera;
    public float minFOV = 40;
    public float maxFOV = 70;
    public float boostFOV = 10;
    public float currentFOV = 40;

    private Vector3 carRight;
    private Vector3 carFwd;
    private Vector3 tempVEC;
    private Vector3 gravityDirection;

    private VehicleInput input;

    private SplinePlus _aiSplineScript;
    private Dictionary<int, Branch> _branchesAtStart = new Dictionary<int, Branch>();
    private Vector3 closestVertex = Vector3.zero;
    public int closestIndex = 0;
    private Vector3 vertexAim = Vector3.zero;
    

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

        if (GetComponent<VehicleInput>())
        {
            RaceManager rc = FindObjectOfType<RaceManager>();
           
            //Sets ai spline to find/follow hotspotspline
            if (rc != null && rc.orderedSplines.Length != 0)
            {
                _aiSplineScript = rc.orderedSplines[0].GetComponent<SplinePlus>();
                _branchesAtStart = new Dictionary<int, Branch>(_aiSplineScript.SPData.DictBranches);
                InvokeRepeating("findNearest", 0, 2);
            }
        }
    }

    public void findNearest()
    {
        float lastDistance = Mathf.Infinity;

        float currentPosition;
        int placeHolder = 0;


        foreach (KeyValuePair<int, Branch> entry in _branchesAtStart)
        {
            for (int j = 0; j < entry.Value.Vertices.Count; j++)
            {
                currentPosition = Vector3.Distance(entry.Value.Vertices[j], transform.position);

                if (closestVertex == Vector3.zero)
                {

                    closestVertex = entry.Value.Vertices[j];
                    closestIndex = j;
                    lastDistance = currentPosition;
                }
                else if (currentPosition <= lastDistance)
                {

                    placeHolder = j;
                    placeHolder = Mathf.Clamp(placeHolder + 5, 0, entry.Value.Vertices.Count - 1);

                    vertexAim = entry.Value.Vertices[placeHolder];

                    lastDistance = currentPosition;
                    closestVertex = vertexAim;
                    closestIndex = placeHolder;
                }

            }
        }
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
        
        if(relativeVel.y < -springFallDisengangeSpeed) setSpringForce(0f);
        else setSpringForce(spring);

        // calculate how much we are sliding (find out movement along our x axis)
        slideSpeed = Vector3.Dot(myRight, flatVel);

        // calculate current speed (the magnitude of the flat velocity)
        currentSpeed = flatVel.magnitude;
        relativeAngularVel = carTransform.InverseTransformDirection(carRigidbody.angularVelocity);
        currentTurnSpeed = Mathf.Abs(relativeAngularVel.y);



        // calculate engine force with our flat direction vector and acceleration
        engineForce = (flatDir * (power * (throttle - reverse)) * carMass);

        if (isBoosting)
        {
            
            engineForce += (flatDir * (power * boostAccelerationMulti )* carMass);
        }

        if(resetBoostTrigger)
        {
            resetBoostTime += Time.deltaTime;
            boostSpeed = Mathf.Lerp(boostSpeed, 0f, (1 / 3f) * resetBoostTime);
            if(boostSpeed <= 0f)
            {
                boostSpeed = 0f;
                resetBoostTime = 0f;
                resetBoostTrigger = false;
            }
        }
        if(resetBoostPadTrigger)
        {
            resetBoostPadTime += Time.deltaTime;
            boostPadSpeed = Mathf.Lerp(boostSpeed, 0f, (1/3f)*resetBoostPadTime);
            if (boostPadSpeed <= 0f)
            {
                boostPadSpeed = 0f;
                resetBoostPadTime = 0f;
                resetBoostPadTrigger = false;
            }
        }

        ///FOV STUFF
        if(input != null)
        {
            currentFOV = (maxFOV - minFOV) * (currentSpeed / maxSpeed) + minFOV;
            if((boostSpeed > 0 || boostPadSpeed> 0))
            {
                currentFOV += boostFOV * (boostSpeed/50) * (currentSpeed / maxSpeed);
            }
            cineCamera.m_Lens.FieldOfView = currentFOV;
            if (boostPadSpeed <= 0f)
            {
                boostPadSpeed = 0f;
                resetBoostPadTime = 0f;
                resetBoostPadTrigger = false;
            }
        }


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
            carRigidbody.AddForce(Vector3.down * gravity * carRigidbody.mass);
        }
        

        if (currentSpeed < maxSpeed + boostSpeed + boostPadSpeed)
        {
            // apply the engine force to the rigidbody
            carRigidbody.AddForce(engineForce * Time.deltaTime);
        }
       else
        {
          cheatPhysics();
        }

        // turn car
        float tempMaxTurnSpeed = maxTurnAngle;
        if(drift)
        {
            tempMaxTurnSpeed *= driftStrength;
        }

        

        if (currentTurnSpeed < tempMaxTurnSpeed || relativeAngularVel.y * horizontal < 0f)
        {
            if(relativeAngularVel.y * horizontal < 0f)
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

        if ((Mathf.Abs(throttle) >= deadZone && Mathf.Abs(reverse) <= deadZone || Mathf.Abs(reverse) >= deadZone && Mathf.Abs(throttle) <= deadZone) || !isCarGrounded() || isBoosting)
        {
            carRigidbody.drag = 0f;
        }
        else
        {
            carRigidbody.drag = deceleration;
        }

        // apply forces to our rigidbody for grip
        carRigidbody.AddForce(imp * Time.deltaTime);


 
        if (Physics.Raycast(transform.position, carUp, out hit, 5f, LayerMask.GetMask("Ground")))
        {
            GetComponent<CarHealthBehavior>().Kill();
        }
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
            //Debug.Log("but why");
            //get relative down velocity vector
            Vector3 downSpeed = transform.InverseTransformDirection(carRigidbody.velocity);
            downSpeed.x = 0;
            downSpeed.z = 0;
            carRigidbody.velocity = ((maxSpeed + boostSpeed + boostPadSpeed) * flatVel.normalized) + transform.TransformDirection(downSpeed);
        }
    }

    public void setBoostSpeed(float percentage)
    {
        boostSpeed = maxSpeed * percentage;
    }

    public void ResetBoostSpeed()
    {
        resetBoostTrigger = true;
    }

    public float getBoostSpeed()
    {
        return boostSpeed;
    }

    public void SetBoostPadSpeed(float percentage)
    {
        boostPadSpeed = maxSpeed * percentage;
    }

    public void ResetBoostPadSpeed()
    {
        resetBoostPadTrigger = true;
    }

    public Vector3 getClosestVertex()
    {
        return closestVertex;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0, 0, 1, 1);
        Gizmos.DrawRay(transform.position, engineForce.normalized * 15f);
    }

    public Vector3 GetNearestPointOnSpline(int vectorsBack)
    {
        Vector3 closestWorldPoint = new Vector3();
        float lastDistance = int.MaxValue;
        int vectorsBackAdjustment;
        RaceManager rc = FindObjectOfType<RaceManager>();
        foreach (GameObject spline in rc.orderedSplines)
        {
            foreach (KeyValuePair<int, Branch> entry in spline.GetComponent<SplinePlus>().SPData.DictBranches)
            {
                for (int j = 0; j < entry.Value.Vertices.Count; j++)
                {
                    float distance = Vector3.Distance(entry.Value.Vertices[j], transform.position);
                    if (distance <= lastDistance)
                    {
                        lastDistance = distance;
                        vectorsBackAdjustment = Mathf.Clamp(j + vectorsBack, 1, entry.Value.Vertices.Count - 1);
                        closestWorldPoint = entry.Value.Vertices[vectorsBackAdjustment];
                    }
                }
            }
        }
        
        return closestWorldPoint;
    }
}

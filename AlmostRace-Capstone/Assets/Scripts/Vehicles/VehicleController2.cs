using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleController2 : MonoBehaviour
{
    [Tooltip("List holding the vehicle's wheels")]
    [SerializeField] private Wheel[] _wheels;
    [Tooltip("How far from the center of the vehicle the center of mass is set, Negative Y sets it below the car")]
    [SerializeField] private Vector3 _centerOfMassOffset;
    [SerializeField] private float _maxSteerAngle;
    [Tooltip("Base Torque across all powered wheels. Value affects acceleration and top speed")]
    [SerializeField] private float _acceleration;
    [Tooltip("Force applied to keep car stable and grounded")]
    [SerializeField] private float _downForce;
    [Tooltip("Highest velocity car will reach. It will stop accelerating at this point")]
    [SerializeField] private float _topSpeed;
    [Tooltip("How much the tires can slip before traction control kicks in")]
    [SerializeField] private float _slipLimit;
    [Header("Force used for each behavior")]
    [SerializeField] private float _brakeTorque;
    [SerializeField] private float _handBrakeTorque;
    [SerializeField] private float _reverseTorque;

    private float _accelerationMultiplier = 2;
    private float _handling = 1;

    private Rigidbody _rigidbody;
    private float _currentTorque;
    private float _oldRotation;
    private float _steerAngle;
    private float _heat;
    private float poweredWheels = 0;

    public float brakeInput { get; private set; }
    public float currentSteerAngle { get { return _steerAngle; } }
    public float currentSpeed { get { return _rigidbody.velocity.magnitude; } }
    public float maxSpeed { get { return _topSpeed; } }
    public float heat { get { return _heat; } }
    public float accelInput { get; private set; }

    private void Start()
    {
        //Assigns the given acceleration and handling values to the forward
        //and sideways stiffness values respectively
        WheelFrictionCurve fFriction = _wheels[0].collider.forwardFriction;
        fFriction.stiffness = _accelerationMultiplier;

        WheelFrictionCurve sFriction = _wheels[0].collider.sidewaysFriction;
        sFriction.stiffness = _handling;

        _wheels[0].collider.attachedRigidbody.centerOfMass = _centerOfMassOffset;

        //calculates how many wheels are powered for later use, and assigns
        //appropriate friction curves
        for (int i = 0; i < _wheels.Length; i++)
        {
            if (_wheels[i].isPowered) poweredWheels++;
            _wheels[i].collider.forwardFriction = fFriction;
            _wheels[i].collider.sidewaysFriction = sFriction;
        }

        _rigidbody = GetComponent<Rigidbody>();
    }

    //Takes user input from the VehicleInput script and uses it here
    public void Move(float steer, float accel, float brake, float handbrake)
    {
        if (currentSpeed >= 100)
        {
            if (true)
            {

            }
        }
        //clamp input values
        steer = Mathf.Clamp(steer, -1, 1);
        accelInput = accel = Mathf.Clamp(accel, 0, 1);
        brakeInput = brake = -1 * Mathf.Clamp(brake, -1, 0);
        handbrake = Mathf.Clamp(handbrake, 0, 1);

        //Gets the current steering input and assigns it to the
        //steerable wheels
        _steerAngle = steer * _maxSteerAngle;
        for (int i = 0; i < _wheels.Length; i++)
        {
            if (_wheels[i].isSteerable)
            {
                _wheels[i].collider.steerAngle = _steerAngle;
            }
        }

        if (Mathf.Abs(_steerAngle) > 1.0f && currentSpeed > _topSpeed && handbrake == 0)
        {
            print(currentSpeed);
            accel = 0;
        }

        //Calls methods to stabilize the car to prevent spin outs, 
        //apply any forward or backward forces from the user, and
        //to cap the vehicle's velocity if necessary
        SteerControl();
        ApplyDrive(accel, brake);
        CapSpeed();

        //Checks if the handbrake is being pressed, and if so,
        //applies the appropriate force
        if (handbrake > 0f)
        {
            var hbTorque = handbrake * _handBrakeTorque;
            for (int i = 0; i < _wheels.Length; i++)
            {
                if (_wheels[i].hasBrakes)
                {
                    _wheels[i].collider.brakeTorque = hbTorque;

                }
                if (_wheels[i].isPowered && accel == 0)
                {
                    _wheels[i].collider.motorTorque = 0;
                }
            }
        }
        else
        {
            for (int i = 0; i < _wheels.Length; i++)
            {
                if (_wheels[i].hasBrakes)
                {
                    _wheels[i].collider.brakeTorque = 0;

                }
            }
        }
        //Forces are applied to keep the car from spinning out,
        //And to keep the car grounded
        AddDownForce();
        TractionControl();
    }

    //This method prevents the car from shaking wildly and from turning uncontrollably
    public void SteerControl()
    {
        for (int i = 0; i < 4; i++)
        {
            WheelHit wheelhit;
            _wheels[i].collider.GetGroundHit(out wheelhit);
            if (wheelhit.normal == Vector3.zero)
                return; // wheels arent on the ground so dont realign the rigidbody velocity
        }
        _oldRotation = transform.eulerAngles.y;
    }

    //This method is used to apply the motor and braking torques
    //to the appropriate wheels
    public void ApplyDrive(float accel, float brake)
    {
        float thrust = accel * (_currentTorque / poweredWheels);
        for (int i = 0; i < _wheels.Length; i++)
        {

            if (_wheels[i].isPowered)
            {
                _wheels[i].collider.motorTorque = thrust;
            }

            if (currentSpeed > 5 && Vector3.Angle(transform.forward, _rigidbody.velocity) < 50f)
            {
                _wheels[i].collider.brakeTorque = _brakeTorque * brake;
            }
            else if (brake > 0)
            {
                _wheels[i].collider.brakeTorque = 0f;
                _wheels[i].collider.motorTorque = -_reverseTorque * brake;
            }
        }

    }

    //Checks if the user would surpass the speed cap, and lowers
    //their speed to the cap if so.
    public void CapSpeed()
    {
        float speed = _rigidbody.velocity.magnitude;
        if (speed > _topSpeed)
        {
            _rigidbody.velocity = _topSpeed * _rigidbody.velocity.normalized;
        }
    }

    //Applies a downward force to the car that increases with the
    //car's speed
    public void AddDownForce()
    {
        _rigidbody.AddForce(Physics.gravity.normalized * _downForce * _rigidbody.velocity.magnitude);
    }

    //Checks which wheels are both powered and on the ground
    //And uses that info to adjust the torque applied to each
    //wheel
    public void TractionControl()
    {
        WheelHit hit;
        for (int i = 0; i < _wheels.Length; i++)
        {
            if (_wheels[i].isPowered)
            {
                _wheels[i].collider.GetGroundHit(out hit);

            }
        }
    }

    //Checks if the wheel is slipping too much, or if the torque is

}

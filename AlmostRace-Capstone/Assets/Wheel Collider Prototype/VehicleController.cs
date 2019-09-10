using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]

public class Wheel
{
    //The collider this wheel uses
    public WheelCollider collider;

    //Whether this wheel should be powered by the engine
    public bool powered;

    //Whether this wheel is steerable
    public bool steerable;

    //Whether this wheel can apply brakes
    public bool hasBrakes;
}

public class VehicleController : MonoBehaviour
{
    [SerializeField] private Wheel[] _wheels;
    [SerializeField] private Vector3 _centerOfMassOffset;
    [SerializeField] private float _maxSteerAngle;
    [Range(0, 1)] [SerializeField] private float _steerHelper;
    [Range(0, 1)] [SerializeField] private float _tractionControl;
    [SerializeField] private float _fullTorqueOverAllWheels;
    [SerializeField] private float _downForce;
    [SerializeField] private float _topSpeed;
    [SerializeField] private float _slipLimit;
    [SerializeField] private float _brakeTorque;
    [SerializeField] private float _handBrakeTorque;
    [SerializeField] private float _reverseTorque;
    [SerializeField] private float _maxHeat;
    [SerializeField] private float _acceleration;
    [SerializeField] private float _handling;

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
        WheelFrictionCurve fFriction = _wheels[0].collider.forwardFriction;
        fFriction.stiffness = _acceleration;

        WheelFrictionCurve sFriction = _wheels[0].collider.sidewaysFriction;
        sFriction.stiffness = _handling;

        _wheels[0].collider.attachedRigidbody.centerOfMass = _centerOfMassOffset;
        for(int i = 0; i < _wheels.Length; i++)
        {
            if (_wheels[i].powered) poweredWheels++;
            _wheels[i].collider.forwardFriction = fFriction;
            _wheels[i].collider.sidewaysFriction = sFriction;
        }
        _handBrakeTorque = float.MaxValue;

        _rigidbody = GetComponent<Rigidbody>();
        _currentTorque = _fullTorqueOverAllWheels - (_tractionControl * _fullTorqueOverAllWheels);
    }

    public void Move(float steer, float accel, float brake, float handbrake)
    {
        //clamp input values
        steer = Mathf.Clamp(steer, -1, 1);
        accelInput = accel = Mathf.Clamp(accel, 0, 1);
        brakeInput = brake = -1 * Mathf.Clamp(brake, -1, 0);
        handbrake = Mathf.Clamp(handbrake, 0, 1);

        _steerAngle = steer * _maxSteerAngle;
        for(int i = 0; i < _wheels.Length; i++)
        {
            if (_wheels[i].steerable)
            {
                _wheels[i].collider.steerAngle = _steerAngle;
            }
        }

        SteerControl();
        ApplyDrive(accel, brake);
        CapSpeed();

        if(handbrake > 0f)
        {
            var hbTorque = handbrake * _handBrakeTorque;
            for(int i = 0; i < _wheels.Length; i++)
            {
                if (_wheels[i].hasBrakes)
                {
                    _wheels[i].collider.brakeTorque = hbTorque;
                }
            }
        }

        Debug.Log(_rigidbody.velocity.magnitude);

        AddDownForce();
        TractionControl();
    }


    public void SteerControl()
    {
        for (int i = 0; i < 4; i++)
        {
            WheelHit wheelhit;
            _wheels[i].collider.GetGroundHit(out wheelhit);
            if (wheelhit.normal == Vector3.zero)
                return; // wheels arent on the ground so dont realign the rigidbody velocity
        }

        // this if is needed to avoid gimbal lock problems that will make the car suddenly shift direction
        if (Mathf.Abs(_oldRotation - transform.eulerAngles.y) < 10f)
        {
            var turnadjust = (transform.eulerAngles.y - _oldRotation) * _steerHelper;
            Quaternion velRotation = Quaternion.AngleAxis(turnadjust, Vector3.up);
            _rigidbody.velocity = velRotation * _rigidbody.velocity;
        }
        _oldRotation = transform.eulerAngles.y;
    }

    public void ApplyDrive(float accel, float brake)
    {
        float thrust = accel * (_currentTorque/poweredWheels);
        for(int i = 0; i < _wheels.Length; i++)
        {
            if (_wheels[i].powered)
            {
                _wheels[i].collider.motorTorque = thrust;
            }

            if(currentSpeed > 5 && Vector3.Angle(transform.forward,_rigidbody.velocity) < 50f)
            {
                _wheels[i].collider.brakeTorque = _brakeTorque * brake;
            } else if (brake > 0)
            {
                _wheels[i].collider.brakeTorque = 0f;
                _wheels[i].collider.motorTorque = -_reverseTorque * brake;
            }
        }
        
    }

    public void CapSpeed()
    {
        float speed = _rigidbody.velocity.magnitude;
        if(speed > _topSpeed)
        {
            _rigidbody.velocity = _topSpeed * _rigidbody.velocity.normalized;
        }
    }

    public void AddDownForce()
    {
        _rigidbody.AddForce(-transform.up * _downForce * _rigidbody.velocity.magnitude);
    }

    public void TractionControl()
    {
        WheelHit hit;
        for(int i = 0; i < _wheels.Length; i++)
        {
            if (_wheels[i].powered)
            {
                _wheels[i].collider.GetGroundHit(out hit);

                AdjustTorque(hit.forwardSlip);
            }
        }
    }

    public void AdjustTorque(float slip)
    {
        if(slip > _slipLimit && _currentTorque >= 0)
        {
            _currentTorque -= 10 * _tractionControl;
        }
        else
        {
            _currentTorque += 10 * _tractionControl;
            if(_currentTorque > _fullTorqueOverAllWheels)
            {
                _currentTorque = _fullTorqueOverAllWheels;
            }
        }
    }
}


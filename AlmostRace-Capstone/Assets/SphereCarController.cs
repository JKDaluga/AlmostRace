using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(VehicleInput))]
public class SphereCarController : MonoBehaviour
{
    public Transform kartModel;
    public Transform kartNormal;
    public Rigidbody sphere;
    private VehicleInput _vehicleInput;
    public UnityStandardAssets.ImageEffects.TiltShift tiltShift;
    private float _maxBlurArea = 7f;
    private float _blurScaling = 10f;

    float speed, currentSpeed;
    float rotate, currentRotate;

    public LayerMask layerMask;
    public float topSpeed = 30f;
    public float acceleration = 12f;
    public float deceleration = 12f;
    public float steering = 80f;
    public float gravity = 10f;
    public float driftStrength = 1f;
    public float reverseSpeed = 1f;

    private bool _drifting;
    private int _driftDirection = 1;

    private bool _isBoosting = false;
    private float _boostSpeed;

    private void Start()
    {
        _vehicleInput = GetComponent<VehicleInput>();
    }

    // Update is called once per frame
    void Update()
    {

        speed = topSpeed * (Input.GetAxis(_vehicleInput.verticalForward) - Input.GetAxis(_vehicleInput.verticalBackward));
        if (_isBoosting)
        {
            speed = _boostSpeed;
        }


        if (speed < 0)
        {
            speed *= reverseSpeed;
        }
        if (Input.GetButtonUp(_vehicleInput.brake))
        {
            _drifting = false;
        }
        if (Input.GetAxis(_vehicleInput.horizontal) != 0)
        {
            int dir = Input.GetAxis(_vehicleInput.horizontal) > 0 ? 1 : -1;
            float amount = Mathf.Abs(Input.GetAxis(_vehicleInput.horizontal));
            if (Input.GetButtonDown(_vehicleInput.brake) && !_drifting && Input.GetAxis(_vehicleInput.horizontal) != 0)
            {
                _drifting = true;
                _driftDirection = Input.GetAxis(_vehicleInput.horizontal) > 0 ? 1 : -1;
            }
            if (_drifting)
            {
                amount = (_driftDirection == 1) ? ExtensionMethods.Remap(Input.GetAxis(_vehicleInput.horizontal), -1, 1, 0, 1 + driftStrength) : ExtensionMethods.Remap(Input.GetAxis(_vehicleInput.horizontal), -1, 1, 1 + driftStrength, 0);
            }

            if (_drifting)
                Steer(_driftDirection, amount);
            else
                Steer(dir, amount);
        }

        transform.position = sphere.transform.position - new Vector3(0, 0.4f, 0);
        if(Mathf.Abs(speed) >= Mathf.Abs(currentSpeed) || speed * currentSpeed > 0)
        {
            currentSpeed = Mathf.SmoothStep(currentSpeed, speed, Time.deltaTime * acceleration); speed = 0f;
        }
        else
        {
            currentSpeed = Mathf.SmoothStep(currentSpeed, speed, Time.deltaTime * deceleration); speed = 0f;
        }
        currentRotate = Mathf.Lerp(currentRotate, rotate, Time.deltaTime * 4f); rotate = 0f;

        //Motion Blur for car speed right now 7 and 10 are the magic numbers for the effect we are looking for
        tiltShift.blurArea = Mathf.Min(_maxBlurArea * (Mathf.Pow(currentSpeed, _blurScaling) / Mathf.Pow(topSpeed, _blurScaling)), 1);
    }

    private void FixedUpdate()
    {
        if (!_drifting)
        {
            sphere.AddForce(-kartModel.transform.right * currentSpeed, ForceMode.Acceleration);
        }
        else
        {
            sphere.AddForce(transform.forward * currentSpeed, ForceMode.Acceleration);
        }

        sphere.AddForce(Vector3.down * gravity, ForceMode.Acceleration);


        transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, new Vector3(0, transform.eulerAngles.y + currentRotate, 0), Time.deltaTime * 12.5f);


        RaycastHit hitOn;
        RaycastHit hitNear;

        Physics.Raycast(transform.position + (transform.up * .1f), Vector3.down, out hitOn, 1.1f, layerMask);
        Physics.Raycast(transform.position + (transform.up * .1f), Vector3.down, out hitNear, 5.0f, layerMask);


        //Normal Rotation
        kartNormal.up = Vector3.Lerp(kartNormal.up, hitNear.normal, Time.deltaTime * 8.0f);
        kartNormal.Rotate(0, transform.eulerAngles.y, 0);
    }

    public void Steer(int direction, float amount)
    {
        rotate = (steering * direction) * amount;
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position + transform.up, transform.position - (transform.up * 2));
    }


    public void SetIsBoosting(bool ToF)
    {
        _isBoosting = ToF;
    }

    public void SetBoostSpeed(float boostSpeed)
    {
        _boostSpeed = boostSpeed;
    }
}

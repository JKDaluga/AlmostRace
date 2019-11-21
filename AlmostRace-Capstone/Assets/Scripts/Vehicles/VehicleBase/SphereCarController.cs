//Script made by Robyn
//Last Edit Robyn 10/7
//Script is used to control the sphere collider based car. Takes inputs and applies collision physics

/*
 Eddie Borissov
 Edit 10/15/2019 added code for the speedometer UI and for Drifting UI and particle effects.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(VehicleInput))]
public class SphereCarController : MonoBehaviour
{

    [Header("Car Object Holders")]
    public Transform kartModel;
    public Transform kartNormal;
    public Rigidbody sphere;


    private VehicleInput _vehicleInput;
    public UnityStandardAssets.ImageEffects.TiltShift tiltShift;
    private float _maxBlurArea = 7f;
    private float _blurScaling = 10f;


    //Input values and values passed to values
    public float speed, currentSpeed;
    float rotate, currentRotate;

    //What physics layers the collision raycasts can hit
    public LayerMask layerMask;

    //values passed in to the various functions
    public float topSpeed = 30f;
    private float _originalTopSpeed;

    public float acceleration = 12f;
    private float _originalAcceleration;

    public float deceleration = 12f;
    private float _originalDeceleration;

    public float steering = 80f;
    private float _originalSteering;

    public float groundedGravity = 10f;
    public float airborneGravity = 10f;
    public float driftStrength = 1f;
    public float reverseSpeed = 1f;

    private bool _drifting;
    private int _driftDirection = 1;

    private bool _isBoosting = false;
    private float _boostSpeedPercentage;
    private float _boostTopSpeedPercentage;

    private bool _isOnBoosterPad = false;
    private float _boosterPadSpeedPercentage;
    private float _boosterPadTopSpeedPercentage;

    [Header("Drift Ability")]
    // public Image driftButton;
    // public Sprite driftSpriteUp;
    // public Sprite driftSpriteDown;

    [Header("Drift Particles")]
    public GameObject leftDriftParticles;
    public GameObject rightDriftParticles;

    [Header("Boost Particles")]
    public ParticleSystem boostingParticles;

    [Header("Collision Particles")]
    public GameObject leftCollisionParticles;
    public GameObject rightCollisionParticles;

    [Header("Speedometer")]
    public Image speedometerImage;

    [Header("Temporary Sound Stuff")]
    public AudioSource driftSound;
    [Header("Camera Stuff")]
    //Added by Robyn Riley 11/5/19
    //A gameobject Childed to the ModelHolder. Controls camera and aiming direction
    public GameObject aimPos;

    //Call allowing vehicle to take input from player
    private void Start()
    {
        _originalTopSpeed = topSpeed;
        _originalAcceleration = acceleration;
        _originalDeceleration = deceleration;
        _originalSteering = steering;
        //AudioManager.instance.Play("Engine");
        _vehicleInput = GetComponent<VehicleInput>();

        if (leftDriftParticles != null && rightDriftParticles != null) //placed here just so that the BallCar prefab doesn't throw nulls
        {
            leftDriftParticles.SetActive(false);
            rightDriftParticles.SetActive(false);
        }
               
    }

    // Update is called once per frame
    void Update()
    {
        float extraSpeed = 0;
        // If vehicle input is turned off don't listen to inputs
        if (!_vehicleInput.getStatus())
        {
            tiltShift.blurArea = Mathf.Min(_maxBlurArea * (Mathf.Pow(currentSpeed, _blurScaling) / Mathf.Pow(topSpeed, _blurScaling)), 1);
            return;
        }

        //Takes input for forward and reverse movement

        if (_isBoosting)
        {
            extraSpeed += ((_boostSpeedPercentage * topSpeed) / 100);
        }
        if(_isOnBoosterPad)
        {
            extraSpeed += ((_boosterPadSpeedPercentage * topSpeed) / 100);
        }

        speed = (topSpeed + extraSpeed) * (Input.GetAxis(_vehicleInput.verticalForward) - Input.GetAxis(_vehicleInput.verticalBackward));

        //Allows for vehicle to back up and break
        if (speed < 0)
        {
            speed *= reverseSpeed;
        }
        if (Input.GetButtonUp(_vehicleInput.brake))
        {
            _drifting = false;
        }

        //Drifting mechanics
        if (Input.GetAxis(_vehicleInput.horizontal) != 0)
        {
            //values are clamped to a specific usable range
            int dir = Input.GetAxis(_vehicleInput.horizontal) > 0 ? 1 : -1;
            float amount = Mathf.Abs(Input.GetAxis(_vehicleInput.horizontal));

            //Checks necessary conditions for drifting to happen
            if (Input.GetButtonDown(_vehicleInput.brake) && !_drifting && Input.GetAxis(_vehicleInput.horizontal) != 0)
            {
                if (driftSound != null) //placed here just so that the BallCar prefab doesn't throw nulls
                {
                    driftSound.Play();
                }
               
                _drifting = true;
                _driftDirection = Input.GetAxis(_vehicleInput.horizontal) > 0 ? 1 : -1;

                if(leftDriftParticles != null && rightDriftParticles != null) //placed here just so that the BallCar prefab doesn't throw nulls
                {
                    if (_driftDirection == 1)
                    {
                        leftDriftParticles.SetActive(true);
                    }
                    else if (_driftDirection == -1)
                    {
                        rightDriftParticles.SetActive(true);
                    }
                }
                else
                {
                    //placed here just so that the BallCar prefab doesn't throw nulls
                    //Delete this later
                    Debug.Log("Drift Particles are null, please assign them!");
                }
                
            }
            if (_drifting)
            {
               // driftButton.sprite = driftSpriteDown;
                //Remaps the user input values to appropriate amounts to allow drifting
                amount = (_driftDirection == 1) ? ExtensionMethods.Remap(Input.GetAxis(_vehicleInput.horizontal), -1, 1, 0, 1 + driftStrength) : ExtensionMethods.Remap(Input.GetAxis(_vehicleInput.horizontal), -1, 1, 1 + driftStrength, 0);
            }

            //passes in appropriate turning values based on drifting bool
            if (_drifting)
            {
                Steer(_driftDirection, amount);
             
            }          
            else
            {
                if(driftSound!=null)  //placed here just so that the BallCar prefab doesn't throw nulls

                {
                    driftSound.Stop();
                }
                // driftButton.sprite = driftSpriteUp;
                if (leftDriftParticles != null && rightDriftParticles != null) //placed here just so that the BallCar prefab doesn't throw nulls
                {
                    leftDriftParticles.SetActive(false);
                    rightDriftParticles.SetActive(false);
                }
                Steer(dir, amount);
            }
                
        }


        //Ties the vehicle body to the sphere collider
        transform.position = sphere.transform.position - new Vector3(0, 0.4f, 0);

        //Checks if the vehicle should be reversing or not, and evenly increases speed based on that.
        if(Mathf.Abs(speed) >= Mathf.Abs(currentSpeed) || speed * currentSpeed > 0)
        {
            currentSpeed = Mathf.SmoothStep(currentSpeed, speed, Time.deltaTime * acceleration); speed = 0f;
        }
        else
        {
            currentSpeed = Mathf.SmoothStep(currentSpeed, speed, Time.deltaTime * deceleration); speed = 0f;
        }

        //Smoothly changes the vehicle's rotation
        currentRotate = Mathf.Lerp(currentRotate, rotate, Time.deltaTime * 4f); rotate = 0f;

        //Motion Blur for car speed right now 7 and 10 are the magic numbers for the effect we are looking for
        tiltShift.blurArea = Mathf.Min(_maxBlurArea * (Mathf.Pow(currentSpeed, _blurScaling) / Mathf.Pow(topSpeed, _blurScaling)), 1);

        if(speedometerImage != null)
        {
            speedometerImage.fillAmount = currentSpeed / topSpeed; //Speedometer code
        }
        else
        {
            //remove this once UI gets replaced
        }
    }

    private void FixedUpdate()
    {
        // If vehicle input is turned off don't listen to inputs
        if (!_vehicleInput.getStatus())
        {
            return;
        }

        //hitOn/hitNear check and rotate the vehicle body up and down based on direction of the track
        RaycastHit hitOn;
        RaycastHit hitNear;

        Physics.Raycast(transform.position + (transform.up * .1f), Vector3.down, out hitOn, 1.5f, layerMask);
        Physics.Raycast(transform.position + (transform.up * .1f), Vector3.down, out hitNear, 5.0f, layerMask);


        //Next we are getting the difference between down and the direction we will apply gravity so we can similarly adjust how we apply regular movement
        Quaternion forceRotation = new Quaternion();
        if (hitOn.collider != null)
        {
            //calculating the angle in radians then converting it to degrees
            forceRotation = Quaternion.FromToRotation(Vector3.down, -hitOn.normal);
        }

        //Applies force in appropriate direction based on drifting
        if (!_drifting)
        { 
            sphere.AddForce((forceRotation *(-kartModel.transform.right)) * currentSpeed, ForceMode.Acceleration);
        }
        else
        {
            sphere.AddForce((forceRotation * (transform.forward)) * currentSpeed, ForceMode.Acceleration);
        }
        
        if(hitOn.collider != null)
        {
            sphere.AddForce(-hitOn.normal * groundedGravity, ForceMode.Acceleration);
        }
        else
        {
            //Adds a multiplier to gravity to keep the car grounded
            sphere.AddForce(Vector3.down * airborneGravity, ForceMode.Acceleration);

        }

        //Smoothly turns the vehicle
        transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, new Vector3(0, transform.eulerAngles.y + currentRotate, 0), Time.deltaTime * 12.5f);

        //gets the vehicle's velocity without upward and downward directions
        Vector3 flatVel = new Vector3(sphere.velocity.x, 0, sphere.velocity.z);


        RaycastHit colliding1, colliding2;

        //Raycasts in the direction of the vehicle's velocity to check for collisions
        Physics.Raycast(sphere.transform.position, flatVel, out colliding1, 5.0f, layerMask);

        if (colliding1.collider != null)
        {
            //checks if the vehicle will be redirected into another wall, and stops the vehicle if it will
            if (Physics.Raycast(sphere.transform.position, Vector3.ProjectOnPlane(sphere.velocity, colliding1.normal), out colliding2, 4.0f, layerMask))
            {
                if (Physics.Raycast(sphere.transform.position, Vector3.ProjectOnPlane(sphere.velocity, colliding2.normal), layerMask))
                {
                    sphere.velocity = Vector3.zero;
                }
            }
            else
            {
                //redirects the vehicle based on collision direction
                sphere.velocity = Vector3.ProjectOnPlane(sphere.velocity, colliding1.normal);
            }
        }

        //Normal Rotation
        //Rotates the vehicle model to be parallel to the ground
        if (hitNear.collider != null)
        {
            kartNormal.up = Vector3.Lerp(kartNormal.up, hitNear.normal, Time.deltaTime * 8.0f);
            kartNormal.Rotate(0, transform.eulerAngles.y, 0);
        } else
        {
            kartNormal.up = Vector3.Lerp(kartNormal.up, sphere.velocity.normalized/2, Time.deltaTime);
            kartNormal.Rotate(0, transform.eulerAngles.y, 0);
        }
    }

    //Takes input for turning, which will be passed to smoothly rotate
    public void Steer(int direction, float amount)
    {
        rotate = (steering * direction) * amount;
    }
    

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(sphere.transform.position, sphere.velocity.normalized * 5.0f);
    }

    public void SetIsBoosting(bool ToF)
    {
        _isBoosting = ToF;
    }

    public void SetBoostInfo(float boostSpeedPercentage)
    {
        _boostSpeedPercentage = boostSpeedPercentage;
    }

    public void SetIsOnBoosterPad(bool ToF)
    {
        _isOnBoosterPad = ToF;
    }

    public void SetBoosterPadInfo(float boosterPadSpeedPercentage)
    {
        _boosterPadSpeedPercentage = boosterPadSpeedPercentage;
    }

    public bool getDrifting()
    {
        return _drifting;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoostBehavior : MonoBehaviour
{
    public int boostTime;
    public float boostAmount;
    public float accelerationAdd;
    public float boostTurnForce;
    private float boostCharge;
    private float iniAccelaration;
    private float iniSpeed;
    private float iniTurnForce;
    public bool canBoost;
    private CarPhysicsBehavior carScript;
    private CarHeatManager carHeatInfo;
    

    //string for controller support
    public string boostAxis;

    //gameObject to access specific camera for each player
    public Camera camera;

    //temporary object enabled/disabled based on boost state
    public GameObject boostParticleEffect;

    // Start is called before the first frame update
    void Start()
    {
        // Sets variables for gameplay
        canBoost = true;
        carHeatInfo = GetComponent<CarHeatManager>();
        carScript = GetComponent<CarPhysicsBehavior>();
        iniAccelaration = carScript.acceleration;
        iniSpeed = carScript.driveForce;
        iniTurnForce = carScript.turnForce;
        boostCharge = iniSpeed + boostAmount;
        boostParticleEffect.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        // Checks for Boost Input and calls corutine if possible
        if (Input.GetButtonDown(boostAxis) && canBoost && (carHeatInfo.heatCurrent < carHeatInfo.heatStallLimit))
        {
            canBoost = false;
            StartCoroutine(Boost());
        }
        if (canBoost == false)
        {
            // Lerps the camera follow speed to be slower
            camera.fieldOfView = Mathf.Lerp(camera.fieldOfView, 90, 0.1f);
        }
        if (canBoost == true)
        {
            // Lerps the camera follow speed to be faster
            camera.fieldOfView = Mathf.Lerp(camera.fieldOfView, 60, 0.1f);
        }
    }
    
    private IEnumerator Boost()
    {
        boostParticleEffect.SetActive(true);

        // Adds the vehicles accelerationa amount
        carScript.acceleration = accelerationAdd;
        carScript.driveForce = boostCharge;
        carScript.turnForce = boostTurnForce;

        // Time while max boost is active
        
        yield return new WaitForSeconds(boostTime);
        boostParticleEffect.SetActive(false);

        // Subtracts the vehicles accelerationa amount
        carScript.acceleration = iniAccelaration;
        carScript.driveForce = iniSpeed;
        carScript.turnForce = iniTurnForce;

        canBoost = true;
    }
}

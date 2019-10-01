﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class CarHeatManager : MonoBehaviour
{
    public Image heatImage;
    public float heatCurrent = 0f;
    public float heatStallLimit = 100f;
    public float heatExplodeLimit = 120f;
    public float cooldownRate = 1f;
    public float respawnSecs = 3f;
    public GameObject explosion;


    private void Start()
    {
        explosion.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))//for testing, heats up car
        {
            heatCurrent = 110f;
        }
        else if (Input.GetKeyDown(KeyCode.N)) // for testing, cools down car
        {
            heatCurrent = 0f;
        }
        else if (Input.GetKeyDown(KeyCode.Y))
        {
            heatCurrent = 120f;
        }

        if (heatCurrent > heatExplodeLimit)
        {
            heatCurrent = heatExplodeLimit;
        }

        if (heatCurrent >= heatExplodeLimit)
        {

            respawn();

        }


        if (heatCurrent > 0)
        {
            heatCurrent -= cooldownRate * Time.deltaTime;
        }
        else if (heatCurrent < 0)
        {
            heatCurrent = 0;
        }


        if (heatCurrent == 0 && gameObject.activeSelf == false)
        {
            gameObject.SetActive(true);
        }

        heatImage.fillAmount = ((heatCurrent * 100) / 120) / 100;

    }


    private void respawn()
    {

        if (GetComponent<VehicleAbilityBehavior>().isActiveAndEnabled)
        {
            GetComponent<BasicAbility>().DeactivateAbility();
            GetComponent<VehicleAbilityBehavior>().enabled = false;

            childCare(true, "Explosion");

            Invoke("respawn", respawnSecs);
        }
        else if (GetComponent<VehicleAbilityBehavior>().isActiveAndEnabled != true)
        {
            heatCurrent = 0;

            GetComponent<VehicleAbilityBehavior>().enabled = true;

            childCare(false, "Explosion");

            GetComponent<BasicAbility>().DeactivateAbility();
        }

    }


    //Change name pls
    public void childCare(bool onOff, string childName)
    {

        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            var child = gameObject.transform.GetChild(i).gameObject;
            if (transform.Find(childName).gameObject.Equals(child))
            {
                child.SetActive(onOff);
            }
            else if (child != null)
            {
                child.SetActive(!onOff);
            }

        }

    }
}


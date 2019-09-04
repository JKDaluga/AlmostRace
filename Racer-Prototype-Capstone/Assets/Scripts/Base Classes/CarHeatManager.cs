using System.Collections;
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


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))//for testing, heats up car
        {
            heatCurrent = 110f;
        }
        else if (Input.GetKeyDown(KeyCode.C)) // for testing, cools down car
        {
            heatCurrent = 0f;
        }
        else if(Input.GetKeyDown(KeyCode.E))
        {
            heatCurrent = 120f;
        }

        if(heatCurrent > 0)
        {
            heatCurrent -=  cooldownRate * Time.deltaTime;
        }
        else if(heatCurrent < 0)
        {
            heatCurrent = 0;
        }

        if (heatCurrent > heatExplodeLimit)
        {
            heatCurrent = heatExplodeLimit;
        }
       
        heatImage.fillAmount = ((heatCurrent * 100) / 120) /100;
     //   Debug.Log("Fillamount: " + heatImage.fillAmount);
    }
}

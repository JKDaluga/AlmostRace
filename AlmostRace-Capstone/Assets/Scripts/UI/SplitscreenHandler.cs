using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
    Creator and developer of script: Leonardo Caballero
*/
public class SplitscreenHandler : MonoBehaviour
{

    private VehicleInput[] arrP;
    private Rect ratio;

    void Start()
    {
        arrP = GameObject.FindObjectsOfType<VehicleInput>();

        foreach (VehicleInput p in arrP)
        {
            VehicleInput v = p.gameObject.GetComponent<VehicleInput>();
            if (p.gameObject.GetComponent<SphereCarController>().tiltShift.gameObject != null)
            {
                Camera c = p.gameObject.GetComponent<SphereCarController>().tiltShift.gameObject.GetComponent<Camera>();
                if (arrP.Length == 2)
                {
                    setUp_2(v.getPlayerNum(), c);
                }
                else if (arrP.Length > 2) {
                    setUp_3_4(v.getPlayerNum(), c);
                }
            }
            
        }
    }


    //Find time to make shorter, better code. 
    private void setUp_2(int num, Camera c)
    {
        if(num == 1)
        {
            ratio = c.rect;
            ratio.width = 1f;
            ratio.height = 0.5f;
            ratio.y = 0.5f;
            ratio.x = 0f;
            c.rect = ratio;
        }
        else if(num == 2){
            ratio = c.rect;
            ratio.width = 1f;
            ratio.height = 0.5f;
            ratio.x = 0f;
            ratio.y = 0f;
            c.rect = ratio;
        }
    }
    private void setUp_3_4(int num, Camera c)
    {
        if (num == 1)
        {
            ratio = c.rect;
            ratio.width = 0.5f;
            ratio.height = 0.5f;
            ratio.y = 0.5f;
            ratio.x = 0f;
            c.rect = ratio;
        }
        else if (num == 2)
        {
            ratio = c.rect;
            ratio.width = 0.5f;
            ratio.height = 0.5f;
            ratio.x = 0.5f;
            ratio.y = 0.5f;
            c.rect = ratio;
        }
        else if (num == 3)
        {
            ratio = c.rect;
            ratio.width = 0.5f;
            ratio.height = 0.5f;
            ratio.x = 0f;
            ratio.y = 0f;
            c.rect = ratio;
        }
        else if (num == 4)
        {
            ratio = c.rect;
            ratio.width = 0.5f;
            ratio.height = 0.5f;
            ratio.x = 0.5f;
            ratio.y = 0f;
            c.rect = ratio;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * Robyn Riley. Code that handles crosshair placement on screens
 * Accesses the Aim assistant object to place the reticle object in the correct screen space location
 */

public class crosshairAim : MonoBehaviour
{
    public AimAssistant aim;
    public Camera cam;

    public Sprite normal, locked;
    
    void FixedUpdate()
    {
        //If the vehicle has a target, assign the reticle to the position and switch the sprite to locked on
            if (aim.target != null && aim.gameObject.GetComponent<AimAssistant>().aimOn)
            {
                Vector3 pos = cam.WorldToScreenPoint(aim.target.GetComponent<Collider>().bounds.center);
                Vector3 offsetPos = new Vector3(pos.x, pos.y + 25, pos.z);
                transform.position = pos;

                GetComponent<Image>().sprite = locked;
            }
            //Otherwise, lock the cursor to the center of the screen.
            else
            {
                transform.position = cam.WorldToScreenPoint(aim.gameObject.GetComponent<AimAssistant>().aimObject.transform.position);
                GetComponent<Image>().sprite = normal;
            }
        
    }


   
}

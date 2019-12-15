using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class crosshairAim : MonoBehaviour
{
    public AimAssistant aim;
    public Camera cam;

    public Sprite normal, locked;
    

    // Update is called once per frame
    void FixedUpdate()
    {
        if(aim.target != null)
        {
            Vector3 pos = cam.WorldToScreenPoint(aim.target.GetComponent<Collider>().bounds.center);
            Vector3 offsetPos = new Vector3(pos.x, pos.y + 25, pos.z);
            transform.position = pos;

            GetComponent<Image>().sprite = locked;
        }
        else
        {
            transform.position = cam.WorldToScreenPoint(aim.gameObject.GetComponent<SphereCarController>().aimObject.transform.position);
            GetComponent<Image>().sprite = normal;
        }
    }


   
}

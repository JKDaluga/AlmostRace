using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Detection : MonoBehaviour
{

    private PlayerInput input;
    private VirtualMouse mouse;
    private GraphicRaycaster gr;
    private PointerEventData pointerEventData = new PointerEventData(null);
    // Start is called before the first frame update
    void Start()
    {
        mouse = GetComponentInParent<VirtualMouse>();
        input = GetComponentInParent<PlayerInput>();
        gr = GetComponentInParent<GraphicRaycaster>();
        
    }

    // Update is called once per frame
    void Update()
    {
        pointerEventData.position = gameObject.transform.position;
        List<RaycastResult> results = new List<RaycastResult>();
        gr.Raycast(pointerEventData, results);
        

        if (Input.GetButtonDown(input.selectButton) == true && results.Count > 0)
        { 

            if (results[0].gameObject.GetComponentInParent<Display>() == true && results[0].gameObject.GetComponentInParent<Display>().gameObject == mouse.Grid)
            {
                mouse.currentVehicle = 0;
                mouse._mouse.SetActive(false);
                gameObject.GetComponentInParent<VirtualMouse>().Grid.GetComponent<Display>().changeStatus(false);


            }
            else if(results[0].gameObject.GetComponentInParent<Display>() == true && results[0].gameObject.GetComponentInParent<Display>().gameObject != mouse.Grid)
            {
                int gotNum = results[0].gameObject.GetComponentInParent<Display>().PlayerNum;
                results[0].gameObject.GetComponentInParent<Display>().playerManagement.GetComponent<PlayerManagement>().turnOff(gotNum - 1);
            }
            else if(results[0].gameObject.GetComponent<VehicleData>() == null)
            {

            }
            else
            {
                mouse.currentVehicle = results[0].gameObject.GetComponent<VehicleData>().VehicleNumber;
                mouse.viewCar();
                gameObject.GetComponentInParent<VirtualMouse>().changeReady(true);
                mouse._mouse.SetActive(false);
                gameObject.GetComponentInParent<VirtualMouse>().Grid.GetComponent<Display>().addedCar(true);
            }
        }
    }


    public void turnOff()
    {
        mouse.currentVehicle = 0;
        
        gameObject.GetComponentInParent<VirtualMouse>().Grid.GetComponent<Display>().changeStatus(false);
    }
}

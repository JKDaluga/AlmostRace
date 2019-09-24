using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereCarController : MonoBehaviour
{
    public Transform kartModel;
    public Transform kartNormal;
    public Rigidbody sphere;

    float speed, currentSpeed;
    float rotate, currentRotate;

    public LayerMask layerMask;
    public float acceleration = 30f;
    public float steering = 80f;
    public float gravity = 10f;

    bool drifting;
    

    // Update is called once per frame
    void Update()
    {
        speed = acceleration * Input.GetAxis("VerticalForwardP1");

        if(Input.GetAxis("HorizontalP1") != 0)
        {
            int dir = Input.GetAxis("HorizontalP1") > 0 ? 1 : -1;
            float amount = Mathf.Abs(Input.GetAxis("HorizontalP1"));
            Steer(dir, amount);
        }

        transform.position = sphere.transform.position - new Vector3(0, 0.4f, 0);

        currentSpeed = Mathf.SmoothStep(currentSpeed, speed, Time.deltaTime * 12f); speed = 0f;
        currentRotate = Mathf.Lerp(currentRotate, rotate, Time.deltaTime * 4f); rotate = 0f;

        
    }

    private void FixedUpdate()
    {
        if (!drifting)
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
}

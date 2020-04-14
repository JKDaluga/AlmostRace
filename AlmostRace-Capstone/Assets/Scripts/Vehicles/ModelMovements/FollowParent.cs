using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowParent : MonoBehaviour
{
    public Transform target;
    public Rigidbody rb;
    public RaycastCar raycastCarScript;
    public float moveSpeed = 10;
    public float rotationSpeed = 10;
    public float stopDistance = 7;
    public float stopParentSpeed = 12;
    private float _distance;
    private float _yRotation;

    void Start()
    {
        transform.position = target.position;
        transform.rotation = target.rotation;
    }

    void FixedUpdate()
    {
        // Rotation Management
        Vector3 vectorToTarget = target.position - transform.position;
        float angle = Mathf.Atan2(vectorToTarget.x, vectorToTarget.z) * Mathf.Rad2Deg;
        Quaternion q = Quaternion.AngleAxis(angle, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, q, Time.deltaTime * rotationSpeed);

        // Follow Management
        _distance = Vector3.Distance(target.position, transform.position);
        if (_distance > stopDistance || raycastCarScript.currentSpeed > stopParentSpeed)
        {
            rb.velocity = (target.position - transform.position) * moveSpeed;
            transform.rotation = Quaternion.LookRotation(target.position - transform.position, target.up);
        }
        else
        {
            transform.position = target.position;
            transform.rotation = target.rotation;
            rb.velocity = new Vector3(0f,0f,0f);
            rb.angularVelocity = new Vector3(0f,0f,0f);
        }
    }

    void Update()
    {
        if (_distance > 100)
        {
            transform.position = target.position;
            transform.rotation = target.rotation;
        }
    }
}

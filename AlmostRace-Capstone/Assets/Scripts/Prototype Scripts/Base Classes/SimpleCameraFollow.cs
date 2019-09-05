using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleCameraFollow : MonoBehaviour
{

    public Transform objectToFollow;
    public Vector3 offest;
    public float followSpeed;
    public float lookSpeed;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        LookAtTarget();
        MoveToTarget();
    }

    public void LookAtTarget() {

        Vector3 lookDirection = objectToFollow.position - transform.position;
        Quaternion rot = Quaternion.LookRotation(lookDirection, Vector3.up);
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, lookSpeed * Time.deltaTime);
    }

    public void MoveToTarget() {

        Vector3 targetPos = objectToFollow.position +
                            objectToFollow.forward * offest.z +
                            objectToFollow.right * offest.x +
                            objectToFollow.up * offest.y;

        transform.position = Vector3.Lerp(transform.position, targetPos, followSpeed * Time.deltaTime);
    }
}

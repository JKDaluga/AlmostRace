using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateSelection : MonoBehaviour
{
    public float speed = 300f;
    private Vector3 destination;
    private bool isSwitching;

    public bool GetSwitching()
    {
        return isSwitching;
    }

    public IEnumerator SwitchPlane(bool isRight)
    {
        isSwitching = true;
        if (isRight)
        {
            if (transform.localEulerAngles.z >= 270)
            {
                destination = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, 0.1f);
                while(transform.eulerAngles.z > destination.z)
                {
                    transform.Rotate (Vector3.forward * (speed * Time.fixedDeltaTime));
                    yield return null;
                }
                transform.eulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, 0);
            }
            else
            {
                destination = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, transform.localEulerAngles.z + 90f);
                while(transform.eulerAngles.z < destination.z)
                {
                    transform.Rotate (Vector3.forward * (speed * Time.fixedDeltaTime));
                    yield return null;
                }
                transform.eulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, destination.z);
            }
        }
        if (!isRight)
        {
            if (transform.localEulerAngles.z <= 5 && transform.localEulerAngles.z >= 0 )
            {
                destination = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, 270f);
                while(transform.eulerAngles.z < destination.z)
                {
                    transform.Rotate (Vector3.back * (speed * Time.fixedDeltaTime));
                    yield return null;
                }
                while(transform.eulerAngles.z > destination.z)
                {
                    transform.Rotate (Vector3.back * (speed * Time.fixedDeltaTime));
                    yield return null;
                }
                transform.eulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, destination.z);
            }
            else
            {
                destination = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, transform.localEulerAngles.z - 90f);
                while(transform.eulerAngles.z > destination.z)
                {
                    transform.Rotate (Vector3.back * (speed * Time.fixedDeltaTime));
                    yield return null;
                }
                transform.eulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, destination.z);
            }
        }
        isSwitching = false;
    }
}

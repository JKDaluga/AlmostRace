using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateSelection : MonoBehaviour
{
    readonly float speed = 150;
    readonly float margin = 5;
    private Vector3 destination;
    private bool isSwitching;
    private bool isRight;

    public void SetSwitching(bool givenSwitch)
    {
        isSwitching = givenSwitch;
        if (isRight)
        {
            if (transform.localEulerAngles.y < 270)
            {
                destination = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y + 90f, transform.localEulerAngles.z);
            }
            else
            {
                destination = new Vector3(transform.localEulerAngles.x, 0, transform.localEulerAngles.z);
            }
        }
        else if (!isRight)
        {
            if (transform.localEulerAngles.y <= margin && transform.localEulerAngles.y >= 0)
            {
                destination = new Vector3(transform.localEulerAngles.x, 270f, transform.localEulerAngles.z);
            }
            else
            {
                destination = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y - 90f, transform.localEulerAngles.z);
            }
        }
    }

    public bool GetSwitching()
    {
        return isSwitching;
    }

    public void SetRightOrLeft(bool set)
    {
        isRight = set;
    }

    private void FixedUpdate()
    {
        Debug.Log(transform.eulerAngles.y);
        if (isSwitching)
        {
            if (isRight)
            {
                transform.Rotate (Vector3.up * (speed * Time.deltaTime));
            }
            else if (!isRight)
            {
                transform.Rotate (Vector3.down * (speed * Time.deltaTime));
            }

            if (transform.eulerAngles.y > destination.y - margin && transform.eulerAngles.y < destination.y + margin)
            {
                isSwitching = false;
                transform.eulerAngles = new Vector3(transform.localEulerAngles.x, destination.y, transform.localEulerAngles.z);
            }
        }     
    }
}

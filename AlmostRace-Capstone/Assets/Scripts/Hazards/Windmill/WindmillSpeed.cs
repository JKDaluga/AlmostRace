using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindmillSpeed : MonoBehaviour
{

    private float _laserSpeed;


    void FixedUpdate()
    {
        transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, Mathf.Lerp(transform.localEulerAngles.z, transform.localEulerAngles.z + _laserSpeed, 1));
    }

    public void UpdateBaseSpeed(float baseSpeed)
    {
        _laserSpeed = baseSpeed;
    }
}

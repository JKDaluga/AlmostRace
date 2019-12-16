using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    Creator and developer of script: Jason Daluga
    Purpose: Spin the car in the character select screen
*/
public class RotateCar : MonoBehaviour
{
    float speed = .5f;

    // Update is called once per frame
    void FixedUpdate()
    {
        this.transform.Rotate(speed * new Vector3(0,1,0));
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplinePlusWheel : MonoBehaviour
{
    public SplinePlus SplinePlus;
    public Transform wheel1;
    public Transform wheel2;

    void Update()
    {
        if (SplinePlus.enabled)
        {
            wheel1.rotation = Quaternion.LookRotation(this.transform.forward, this.transform.right);
            wheel2.rotation = Quaternion.LookRotation(this.transform.forward, this.transform.right);
        }
    }
}

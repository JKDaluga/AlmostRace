using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleModel : MonoBehaviour
{

    public Transform sphere;
    // Update is called once per frame
    void Update()
    {
        this.transform.position = sphere.position;
    }
}

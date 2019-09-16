using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(VehicleController))]
public class VehicleInput : MonoBehaviour
{
    private VehicleController _veh;
    private void Awake()
    {
        _veh = GetComponent<VehicleController>();
    }

    private void FixedUpdate()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        float hb = Input.GetAxis("Jump");
        _veh.Move(h, v, v, hb);
    }
}

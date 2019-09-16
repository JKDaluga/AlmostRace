using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(VehicleController))]
public class VehicleInput : MonoBehaviour
{
    public bool isPlayer1;
    private VehicleController _veh;
    private string _playerNum;
    private void Awake()
    {
        _veh = GetComponent<VehicleController>();
        _playerNum = (isPlayer1) ? "P1" : "P2";
    }

    private void FixedUpdate()
    {
        float h = Input.GetAxis("Horizontal" + _playerNum);
        float v = Input.GetAxis("VerticalForward" + _playerNum);
        float hb = Input.GetAxis("Jump");
        _veh.Move(h, v, v, hb);
    }
}

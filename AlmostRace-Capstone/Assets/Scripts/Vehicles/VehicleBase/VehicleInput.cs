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
        float steering = Input.GetAxis("Horizontal" + _playerNum);
        float drive = Input.GetAxis("VerticalForward" + _playerNum);
        float reverse = Input.GetAxis("VerticalBackwards" + _playerNum);
        float handbrake = Input.GetAxis("Brake" + _playerNum);
        _veh.Move(steering, drive, reverse, handbrake);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(VehicleController))]
public class VehicleInput : MonoBehaviour
{
    public bool isPlayer1;
    private VehicleController _veh;
    private string _playerNum;
    private string _horizontalName;
    private string _vertForwardName;
    private string _vertBackwardName;
    private string _brakeName;

    private void Awake()
    {
        _veh = GetComponent<VehicleController>();
        _playerNum = (isPlayer1) ? "P1" : "P2";

        #if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
            _horizontalName = "Horizontal";
            _vertForwardName = "VerticalForward";
            _vertBackwardName = "VerticalBackwards";
            _brakeName = "Brake"
        #endif

        #if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
            
        #endif
    }

    private void FixedUpdate()
    {

        float steering = Input.GetAxis("Horizontal" + _playerNum);
        float drive = Input.GetAxis("VerticalForward" + _playerNum);
        float reverse = Input.GetAxis("VerticalBackwards" + _playerNum);
        float handbrake = Input.GetAxis("Brake");
        _veh.Move(steering, drive, reverse, handbrake);
    }
}

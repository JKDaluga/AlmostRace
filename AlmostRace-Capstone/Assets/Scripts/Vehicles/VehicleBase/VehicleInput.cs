using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleInput : MonoBehaviour
{
    public bool isPlayer1 = true;

    private string _playerNum;

    public string horizontal { get { return _horizontalName; } }
    private string _horizontalName;

    public string verticalForward { get { return _vertForwardName; } }
    private string _vertForwardName;

    public string verticalBackward { get { return _vertBackwardName; } }
    private string _vertBackwardName;

    public string brake { get { return _brakeName; } }
    private string _brakeName;

    private void Awake()
    {
        _playerNum = (isPlayer1) ? "P1" : "P2";

        #if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
            _horizontalName = "Horizontal" + _playerNum;
            _vertForwardName = "VerticalForward" + _playerNum;
            _vertBackwardName = "VerticalBackwards" + _playerNum;
            _brakeName = "Brake" + _playerNum;
        #endif

        #if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
            
        #endif
    }
}
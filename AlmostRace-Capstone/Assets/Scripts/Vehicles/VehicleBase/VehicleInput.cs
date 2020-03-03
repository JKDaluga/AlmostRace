using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    /*
    Author: Jason Daluga & Jake Velicer
    Purpose: Assigns multiple input variables based on which platform the game is running on.
    Multiple scripts obtain input from these variables based on getters.

    Some edits by Leonardo Caballero to help make the player(s) not move during the starting countdown. 
    */

public class VehicleInput : MonoBehaviour
{

    [Tooltip("They are either player 1, 2, 3, or 4? etc.")] public int playerNumber = 1;

    public string horizontal { get { return _horizontalName; } }
    private string _horizontalName;

    public string verticalForward { get { return _vertForwardName; } }
    private string _vertForwardName;

    public string verticalBackward { get { return _vertBackwardName; } }
    private string _vertBackwardName;

    public string brake { get { return _brakeName; } }
    private string _brakeName;

    public string basicAbilityInput { get {return _basicAbilityName; } }
    private string _basicAbilityName;

    public string signatureAbilityInput { get {return _signatureAbilityName; } }
    private string _signatureAbilityName;

    public string pickupInput { get {return _pickupAbilityName; } }
    private string _pickupAbilityName;

    public string rightHorizontal { get { return _rightHorizontalName; } }
    private string _rightHorizontalName;

    public string rightVertical { get { return _rightVerticalName; } }
    private string _rightVerticalName;

    public string rightStickButton { get { return _rightStickButtonName; } }
    private string _rightStickButtonName;

    public string respawn { get { return _respawn; } }
    private string _respawn;

    private string[] _inputNum = new string[4] { "P1", "P2", "P3", "P4"};

    [SerializeField] private bool activeStatus = false;

    private void Awake()
    {
        setPlayerNum(playerNumber);
    }


    public bool getStatus()
    {
        return activeStatus;
    }

    public void setStatus(bool act)
    {
        activeStatus = act;
    }

    public int getPlayerNum()
    {
        return playerNumber;
    }

    public void setPlayerNum(int num)
    {
        playerNumber = num;

#if !UNITY_EDITOR_OSX && !UNITY_STANDALONE_OSX
        _horizontalName = "Horizontal" + _inputNum[playerNumber - 1];
        _vertForwardName = "VerticalForwards" + _inputNum[playerNumber - 1];
        _vertBackwardName = "VerticalBackwards" + _inputNum[playerNumber - 1];
        _brakeName = "Brake" + _inputNum[playerNumber - 1];
        _basicAbilityName = "BasicAbility" + _inputNum[playerNumber - 1];
        _signatureAbilityName = "SignatureAbility" + _inputNum[playerNumber - 1];
        _pickupAbilityName = "Pickup" + _inputNum[playerNumber - 1];
        _respawn = "Respawn" + _inputNum[playerNumber - 1];
        _rightVerticalName = "RightVertical" + _inputNum[playerNumber - 1];
        _rightHorizontalName = "RightHorizontal" + _inputNum[playerNumber - 1];
        _rightStickButtonName = "RightStickButton" + _inputNum[playerNumber - 1];
#endif

#if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
            _horizontalName = "Horizontal" + _inputNum[playerNumber - 1];
            _vertForwardName = "VerticalForwards" + _inputNum[playerNumber - 1] + "Mac";
            _vertBackwardName = "VerticalBackwards" + _inputNum[playerNumber - 1] + "Mac";
            _brakeName = "Brake" + _inputNum[playerNumber - 1] + "Mac";
            _basicAbilityName = "BasicAbility" + _inputNum[playerNumber - 1] + "Mac";
            _signatureAbilityName = "SignatureAbility" + _inputNum[playerNumber - 1] + "Mac";
            _pickupAbilityName = "Pickup" + _inputNum[playerNumber - 1] + "Mac";
            _respawn = "Respawn" +  _inputNum[playerNumber - 1] + "Mac";
            _rightVerticalName = "RightVertical" + _inputNum[playerNumber - 1] + "Mac";
            _rightHorizontalName = "RightHorizontal" + _inputNum[playerNumber - 1] + "Mac";
            _rightStickButtonName = "RightStickButton" + _inputNum[playerNumber - 1] + "Mac";
#endif
    }
}
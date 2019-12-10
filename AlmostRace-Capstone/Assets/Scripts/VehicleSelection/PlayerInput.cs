using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    [Tooltip("They are either player 1, 2, 3, or 4? etc.")] public int playerNumber = 1;

    public string horizontal { get { return _horizontalName; } }
    private string _horizontalName;

    public string verticalForward { get { return _vertForwardName; } }
    private string _vertForwardName;

    public string verticalBackward { get { return _vertBackwardName; } }
    private string _vertBackwardName;

    public string selectButton { get { return _selectButton; } }
    private string _selectButton;

    public string basicAbilityInput { get { return _basicAbilityName; } }
    private string _basicAbilityName;

    public string awakeButton { get { return _awakeButton; } }
    private string _awakeButton;

    public string backButton { get { return _backButton; } }
    private string _backButton;

    public string rightHorizontal { get { return _rightHorizontalName; } }
    private string _rightHorizontalName;

    public string rightVertical { get { return _rightVerticalName; } }
    private string _rightVerticalName;

    public string rightStickButton { get { return _rightStickButtonName; } }
    private string _rightStickButtonName;

    public string respawn { get { return _respawn; } }
    private string _respawn;

    public string vertical { get { return _verticalName; } }
    private string _verticalName;

    private string[] _inputNum = new string[4] { "P1", "P2", "P3", "P4" };

    private bool activeStatus = true;

    private void Awake()
    {

#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
        _horizontalName = "Horizontal" + _inputNum[playerNumber - 1];
        _vertForwardName = "VerticalForwards" + _inputNum[playerNumber - 1];
        _vertBackwardName = "VerticalBackwards" + _inputNum[playerNumber - 1];
        _selectButton = "Brake" + _inputNum[playerNumber - 1];
        _basicAbilityName = "BasicAbility" + _inputNum[playerNumber - 1];
        _awakeButton = "SignatureAbility" + _inputNum[playerNumber - 1];
        _backButton = "Pickup" + _inputNum[playerNumber - 1];
        _rightVerticalName = "RightVertical" + _inputNum[playerNumber - 1];
        _rightHorizontalName = "RightHorizontal" + _inputNum[playerNumber - 1];
        _verticalName = "VerticalUI" + _inputNum[playerNumber - 1];
#endif

#if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
            _horizontalName = "Horizontal" + _inputNum[playerNumber - 1];
            _vertForwardName = "VerticalForwards" + _inputNum[playerNumber - 1] + "Mac";
            _vertBackwardName = "VerticalBackwards" + _inputNum[playerNumber - 1] + "Mac";
            _selectButton = "Brake" + _inputNum[playerNumber - 1] + "Mac";
            _basicAbilityName = "BasicAbility" + _inputNum[playerNumber - 1] + "Mac";
            _awakeButton = "SignatureAbility" + _inputNum[playerNumber - 1] + "Mac";
            _backButton = "Pickup" + _inputNum[playerNumber - 1] + "Mac";
            _rightVerticalName = "RightVertical" + _inputNum[playerNumber - 1] + "Mac";
            _rightHorizontalName = "RightHorizontal" + _inputNum[playerNumber - 1] + "Mac";
#endif
    }


    public int getPlayerNum()
    {
        return playerNumber;
    }

}

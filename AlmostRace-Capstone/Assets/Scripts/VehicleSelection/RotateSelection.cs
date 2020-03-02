using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateSelection : MonoBehaviour
{
    private float _degreesToShift = 90;
    private float _maxDegrees = 270;
    readonly float _speed = 150;
    readonly float _margin = 5;
    private Vector3 _destination;
    private bool _isSwitching;
    private bool _isRight;
    private int _vehicleCount;

    private void Start()
    {
        _vehicleCount = GameObject.FindObjectOfType<SelectionManager>().amountOfSelections;
        switch (_vehicleCount)
        {
            case 1:
                _degreesToShift = 0;
                _maxDegrees = 0;
                break;

            case 2:
                _degreesToShift = 180;
                _maxDegrees = 180;
                break;

            case 3:
                _degreesToShift = 120;
                _maxDegrees = 240;
                break;

            case 4:
                _degreesToShift = 90;
                _maxDegrees = 270;
                break;

            default:
                Debug.LogError("Set a vehicle amount");
                break;
        }
    }

    public void SetSwitching(bool givenSwitch)
    {
        _isSwitching = givenSwitch;
        if (_isRight)
        {
            if (transform.localEulerAngles.y < _maxDegrees)
            {
                _destination = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y + _degreesToShift, transform.localEulerAngles.z);
            }
            else
            {
                _destination = new Vector3(transform.localEulerAngles.x, 0, transform.localEulerAngles.z);
            }
        }
        else if (!_isRight)
        {
            if (transform.localEulerAngles.y <= _margin && transform.localEulerAngles.y >= 0)
            {
                _destination = new Vector3(transform.localEulerAngles.x, _maxDegrees, transform.localEulerAngles.z);
            }
            else
            {
                _destination = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y - _degreesToShift, transform.localEulerAngles.z);
            }
        }
    }

    public bool GetSwitching()
    {
        return _isSwitching;
    }

    public void SetRightOrLeft(bool set)
    {
        _isRight = set;
    }

    private void FixedUpdate()
    {
        if (_isSwitching)
        {
            if (_isRight)
            {
                transform.Rotate (Vector3.up * (_speed * Time.deltaTime));
            }
            else if (!_isRight)
            {
                transform.Rotate (Vector3.down * (_speed * Time.deltaTime));
            }

            if (transform.eulerAngles.y > _destination.y - _margin && transform.eulerAngles.y < _destination.y + _margin)
            {
                _isSwitching = false;
                transform.eulerAngles = new Vector3(transform.localEulerAngles.x, _destination.y, transform.localEulerAngles.z);
            }
        }     
    }
}

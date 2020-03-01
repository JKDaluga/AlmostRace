using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ViewportController : MonoBehaviour
{
    public GameObject PlayerStatus;
    public GameObject activeStatus;
    private GameObject _vehicleSelected;
    private TextMeshProUGUI _text;
    public RotateSelection rotateSelection;
    public SelectionManager selectionManager;
    private PlayerInput _playerInput;
    private bool _confirmation;
    private bool _ready;

    private void Start()
    {
        _playerInput = gameObject.GetComponent<PlayerInput>();
        _text = PlayerStatus.GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        if (Input.GetAxis(_playerInput.horizontal) > 0.1f)
        {
            if(!rotateSelection.GetSwitching())
            {
                rotateSelection.SetRightOrLeft(true);
                rotateSelection.SetSwitching(true);
            }
        }
        else if (Input.GetAxis(_playerInput.horizontal) < 0)
        {
            if(!rotateSelection.GetSwitching())
            {
                rotateSelection.SetRightOrLeft(false);
                rotateSelection.SetSwitching(true);
            }
        }
    }
    
    public void PlayerJoin(bool status, int num = 0)
    {
        if (status == true)
        {
            _text.text = "PLAYER " + num;
            activeStatus.gameObject.SetActive(false);
            activeStatus.gameObject.GetComponent<TextMeshProUGUI>().text = "SELECT A VEHICLE";
        }
        else if (status == false)
        {
            _text.text = "NO PLAYER";
            activeStatus.gameObject.SetActive(true);
            _confirmation = false;
            selectionManager.UpdateReady();
            activeStatus.gameObject.GetComponent<TextMeshProUGUI>().text = "PRESS Y TO JOIN";
        }
    }

    public void VehicleSelect(bool status)
    {
        if (status == true)
        {
            activeStatus.gameObject.SetActive(true);
            activeStatus.gameObject.GetComponent<TextMeshProUGUI>().text = "PRESS A TO CONFIRM";
        }
        else
        {
            _confirmation = false;
            selectionManager.UpdateReady();
            activeStatus.gameObject.SetActive(true);
            activeStatus.gameObject.GetComponent<TextMeshProUGUI>().text = "SELECT A VEHICLE";
        }
    }

    public void ConfirmVehicle(bool status)
    {
        if (status == true)
        {
            activeStatus.GetComponent<TextMeshProUGUI>().text = "READY";
            _confirmation = status;
            selectionManager.UpdateReady();
        }
        else
        {
            _confirmation = status;
            selectionManager.UpdateReady();
            activeStatus.gameObject.GetComponent<TextMeshProUGUI>().text = "PRESS A TO CONFIRM";
        }
    }

    public void SetReady(bool stat)
    {
        _ready = stat;
    }
    public bool GetReady()
    {
        return _ready;
    }

    public void SetConfirmation(bool givenConfirm)
    {
        _confirmation = givenConfirm;
    }

    public bool GetConfirmation()
    {
        return _confirmation;
    }

    public GameObject GetVehicle()
    {
        return _vehicleSelected;
    }

}

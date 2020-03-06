using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ViewportController : MonoBehaviour
{
    [Header("Player Values")]
    public int playerID;
    public int selectedCarID;
    public SelectionManager selectionManager;

    [Header("UI Values")]
    [Space(20)]
    public GameObject playerNumDisplay;
    public GameObject cover;
    public RawImage inactiveImage;
    public RawImage readyImage;
    public GameObject InfoPanelHolder;
    public RawImage[] InfoPanels = new RawImage[4];
    private TextMeshProUGUI _text;

    [Header("Display Vehicle Values")]
    [Space(20)]
    public GameObject vehicleRotationHolder;
    private RotateSelection _rotateSelection;

    private VehicleInput _playerInput;
    private bool _ready;
    private bool _joined;
    private int _vehicleCount;
    private int _selectedInfoPanel;

    private void Start()
    {
        _text = playerNumDisplay.GetComponent<TextMeshProUGUI>();
        _vehicleCount = selectionManager.amountOfSelections;
        _rotateSelection = vehicleRotationHolder.GetComponent<RotateSelection>();

        cover.SetActive(true);
        vehicleRotationHolder.SetActive(false);
        inactiveImage.enabled = true;
        readyImage.enabled = false;
        InfoPanelHolder.SetActive(false);
    }

    private void Update()
    {
        if (_joined && !selectionManager.GetReadyToStart())
        {
            if (!_ready)
            {
                VehicleScroll();
                InfoScroll();

                if (Input.GetButtonDown(_playerInput.selectButton))
                {
                    VehicleSelect(true);
                }
                else if (Input.GetButtonDown(_playerInput.backButton))
                {
                    PlayerJoin(false, null);
                }
            }
            else
            {
                if (Input.GetButtonDown(_playerInput.backButton))
                {
                    VehicleSelect(false);
                }
            }
        }
    }

    private void VehicleScroll()
    {
        if (Input.GetAxis(_playerInput.horizontal) > 0.3f)
        {
            if(!_rotateSelection.GetSwitching())
            {
                _rotateSelection.SetRightOrLeft(true);
                _rotateSelection.SetSwitching(true);
                if (selectedCarID >= _vehicleCount - 1)
                {
                    selectedCarID = 0;
                }
                else
                {
                    selectedCarID = selectedCarID + 1;
                }
            }
        }
        else if (Input.GetAxis(_playerInput.horizontal) < -0.3f)
        {
            if(!_rotateSelection.GetSwitching())
            {
                _rotateSelection.SetRightOrLeft(false);
                _rotateSelection.SetSwitching(true);
                if (selectedCarID <= 0)
                {
                    selectedCarID = _vehicleCount - 1;
                }
                else
                {
                    selectedCarID = selectedCarID - 1;
                }
            }
        }
    }

    private void InfoScroll()
    {
        if(Input.GetButtonDown(_playerInput.bumperRight))
        {
            if (_selectedInfoPanel >= InfoPanels.Length - 1)
            {
                _selectedInfoPanel = 0;
            }
            else
            {
                _selectedInfoPanel = _selectedInfoPanel + 1;
            }
            for (int i = 0; i < InfoPanels.Length; i++)
            {
                if (i == _selectedInfoPanel)
                {
                    InfoPanels[i].enabled = true;
                }
                else
                {
                    InfoPanels[i].enabled = false;
                }
            }
        }
        else if (Input.GetButtonDown(_playerInput.bumperLeft))
        {
            if (_selectedInfoPanel <= 0)
            {
                _selectedInfoPanel = InfoPanels.Length - 1;
            }
            else
            {
                _selectedInfoPanel = _selectedInfoPanel - 1;
            }
            for (int i = 0; i < InfoPanels.Length; i++)
            {
                if (i == _selectedInfoPanel)
                {
                    InfoPanels[i].enabled = true;
                }
                else
                {
                    InfoPanels[i].enabled = false;
                }
            }
        }
    }
    
    public void PlayerJoin(bool status, VehicleInput controllerNumber)
    {
        if (status == true)
        {
            _playerInput = controllerNumber;
            _joined = true;
            _text.text = "PLAYER " + playerID;
            vehicleRotationHolder.SetActive(true);
            inactiveImage.enabled = false;
            InfoPanelHolder.SetActive(true);
            cover.SetActive(false);

        }
        else if (status == false)
        {
            _joined = false;
            _ready = false;
            _text.text = "NO PLAYER";
            cover.SetActive(true);
            vehicleRotationHolder.SetActive(false);
            inactiveImage.enabled = true;
            InfoPanelHolder.SetActive(false);
            selectionManager.UpdateData(playerID, _ready, selectedCarID, 0);
        }
    }

    private void VehicleSelect(bool status)
    {
        if (status == true)
        {
            _ready = true;
            readyImage.enabled = true;
            InfoPanelHolder.SetActive(false);
            selectionManager.UpdateData(playerID, _ready, selectedCarID, _playerInput.GetPlayerNum());
        }
        else
        {
            _ready = false;
            readyImage.enabled = false;
            InfoPanelHolder.SetActive(true);
            selectionManager.UpdateData(playerID, _ready, selectedCarID, _playerInput.GetPlayerNum());
            
        }
    }

    public bool GetReady()
    {
        return _ready;
    }

    public bool GetJoined()
    {
        return _joined;
    }

    public VehicleInput GetInput()
    {
        return _playerInput;
    }

}

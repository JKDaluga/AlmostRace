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
    public GameObject infoPanelHolder;
    public GameObject[] infoPanels = new GameObject[4];
    public GameObject[] vehicleDescriptionText;
    public TextMeshProUGUI selectedVehicleText;
    private TextMeshProUGUI[] _infoText;
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
        _infoText = new TextMeshProUGUI[infoPanels.Length];
        for(int i = 0; i < _infoText.Length; i++)
        {
            _infoText[i] = infoPanels[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        }

        cover.SetActive(true);
        vehicleRotationHolder.SetActive(false);
        inactiveImage.enabled = true;
        readyImage.enabled = false;
        infoPanelHolder.SetActive(false);
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
                AbilityInfoViewState();
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
                AbilityInfoViewState();
            }
        }
    }

    private void InfoScroll()
    {
        if(Input.GetButtonDown(_playerInput.bumperRight))
        {
            if (_selectedInfoPanel >= infoPanels.Length - 1)
            {
                _selectedInfoPanel = 0;
            }
            else
            {
                _selectedInfoPanel = _selectedInfoPanel + 1;
            }
            PanelViewState();
        }
        else if (Input.GetButtonDown(_playerInput.bumperLeft))
        {
            if (_selectedInfoPanel <= 0)
            {
                _selectedInfoPanel = infoPanels.Length - 1;
            }
            else
            {
                _selectedInfoPanel = _selectedInfoPanel - 1;
            }
            PanelViewState();
        }
    }

    private void PanelViewState()
    {
        for (int i = 0; i < infoPanels.Length; i++)
        {
            if (i == _selectedInfoPanel)
            {
                infoPanels[i].SetActive(true);
            }
            else
            {
                infoPanels[i].SetActive(false);
            }
        }
    }

    private void AbilityInfoViewState()
    {
        selectedVehicleText.text = vehicleDescriptionText[selectedCarID].GetComponent<VehicleAbilityDescriptions>().GetVehicleName();
        for (int i = 0; i < _infoText.Length; i++)
        {
            _infoText[i].text = vehicleDescriptionText[selectedCarID].GetComponent<VehicleAbilityDescriptions>().GetSelectecAbilityText(i).text;
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
            infoPanelHolder.SetActive(true);
            AbilityInfoViewState();
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
            infoPanelHolder.SetActive(false);
            selectionManager.UpdateData(playerID, _ready, selectedCarID, 0);
        }
    }

    private void VehicleSelect(bool status)
    {
        if (status == true)
        {
            _ready = true;
            readyImage.enabled = true;
            infoPanelHolder.SetActive(false);
            selectionManager.UpdateData(playerID, _ready, selectedCarID, _playerInput.GetPlayerNum());
        }
        else
        {
            _ready = false;
            readyImage.enabled = false;
            infoPanelHolder.SetActive(true);
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

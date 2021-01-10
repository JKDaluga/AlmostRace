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
    public Camera viewportCamera;
    public Image inactiveImage;
    public Image readyImage;
    public GameObject infoPanelHolder;
    public GameObject[] infoPanels = new GameObject[4];
    public GameObject[] vehicleDescriptionImages;
    private Image[] _infoImage;
    public TextMeshProUGUI selectedVehicleText;
    private TextMeshProUGUI _text;

    [Header("Display Vehicle Values")]
    [Space(20)]
    public GameObject vehicleRotationHolder;
    private RotateSelection _rotateSelection;
    public PlayAbilityExamples[] abilityExampleScript;

    private VehicleInput _playerInput;
    private Animator _camAnimator;
    private bool _ready;
    private bool _joined;
    private int _vehicleCount;
    private int _selectedInfoPanel;
    private bool _switchingPanel;

    private void Start()
    {
        _text = playerNumDisplay.GetComponent<TextMeshProUGUI>();
        _vehicleCount = selectionManager.amountOfSelections;
        _rotateSelection = vehicleRotationHolder.GetComponent<RotateSelection>();
        _infoImage = new Image[infoPanels.Length];
        _camAnimator = viewportCamera.GetComponent<Animator>();
        for(int i = 0; i < _infoImage.Length; i++)
        {
            _infoImage[i] = infoPanels[i].transform.GetChild(0).GetComponent<Image>();
        }

        cover.SetActive(true);
        vehicleRotationHolder.SetActive(false);
        viewportCamera.enabled = false;
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
            }
        }
    }

    private void VehicleScroll()
    {
        if (Input.GetAxis(_playerInput.horizontal) > 0.3f || Input.GetAxis(_playerInput.triggerRight) > 0)
        {
            if(!_rotateSelection.GetSwitching())
            {
                abilityExampleScript[selectedCarID].DeactivateAllAbilites();
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
        else if (Input.GetAxis(_playerInput.horizontal) < -0.3f || Input.GetAxis(_playerInput.triggerLeft) > 0)
        {
            if(!_rotateSelection.GetSwitching())
            {
                abilityExampleScript[selectedCarID].DeactivateAllAbilites();
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
        if (!_switchingPanel)
        {
            if(Input.GetButtonDown(_playerInput.bumperRight) || Input.GetAxis(_playerInput.horizontalDPad) > 0)
            {
                _switchingPanel = true;
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
            else if (Input.GetButtonDown(_playerInput.bumperLeft) || Input.GetAxis(_playerInput.horizontalDPad) < 0)
            {
                _switchingPanel = true;
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
        else 
        {   
            if (Input.GetButtonUp(_playerInput.bumperRight) || Input.GetAxis(_playerInput.horizontalDPad) == 0)
            {
                _switchingPanel = false;
            }
        }
    }

    private void PanelViewState()
    {
        //abilityExampleScript[selectedCarID].DeactivateCurrentAbility();
        for (int i = 0; i < infoPanels.Length; i++)
        {
            if (i == _selectedInfoPanel)
            {
                infoPanels[i].SetActive(true);
                abilityExampleScript[selectedCarID].currentAbilityExample[i] = true;
            }
            else
            {
                infoPanels[i].SetActive(false);
                abilityExampleScript[selectedCarID].currentAbilityExample[i] = false;
            }
        }
    }

    private void AbilityInfoViewState()
    {
        _selectedInfoPanel = 0;
        PanelViewState();
        for (int i = 0; i < _infoImage.Length; i++)
        {
            _infoImage[i].sprite = vehicleDescriptionImages[selectedCarID].GetComponent<VehicleAbilityDescriptions>().GetSelectecAbilityText(i).sprite;
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
            viewportCamera.enabled = true;
            cover.SetActive(false);

        }
        else if (status == false)
        {
            _joined = false;
            _ready = false;
            _text.text = "NO PLAYER";
            cover.SetActive(true);
            abilityExampleScript[selectedCarID].DeactivateAllAbilites();
            //vehicleRotationHolder.SetActive(false);
            viewportCamera.enabled = false;
            inactiveImage.enabled = true;
            infoPanelHolder.SetActive(false);
            _playerInput = null;
            selectionManager.UpdateData(playerID, _ready, selectedCarID, 0);
        }
    }

    public void VehicleSelect(bool status)
    {
        if (status == true)
        {
            _ready = true;
            readyImage.enabled = true;
            infoPanelHolder.SetActive(false);
            _camAnimator.Play("VehicleSelectCameraZoomIn");
            abilityExampleScript[selectedCarID].DeactivateAllAbilites();
            selectionManager.UpdateData(playerID, _ready, selectedCarID, _playerInput.GetPlayerNum());
        }
        else
        {
            _ready = false;
            readyImage.enabled = false;
            infoPanelHolder.SetActive(true);
            _camAnimator.Play("VehicleSelectCameraZoomOut");
            selectionManager.UpdateData(playerID, _ready, selectedCarID, _playerInput.GetPlayerNum());
            
        }
        AudioManager.instance.PlayWithoutSpatial("Menu Selection");
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


/*
    Creator and developer of script: Leonardo Caballero
    Purpose: For character select screen, it allows players to move their "cursor" with their gamepad. 
*/

public class VirtualMouse : UIBehaviour
{ 

    public int currentVehicle;

    private PlayerInput _playerInput;
    public GameObject _mouse;
    public GameObject Grid;
    private bool _ready = false;
    private bool _isInfoOn = false;
    private bool _canSwap = true;
    private int _pageIndex = 0;

    public GameObject[] infoScreens;
    public GameObject[] carModels;

    private bool overlapping = true;

    public int sens = 15;

    protected override void Start()
    {
        _playerInput = GetComponent<PlayerInput>();
    }

    private void Update()
    {
        if (Input.GetButtonDown(_playerInput.awakeButton) == true && _mouse.activeSelf == false && _ready == false)
        {
            _mouse.SetActive(true);
            Grid.GetComponent<Display>().changeStatus(true, _playerInput.GetPlayerNum()); ;
            Grid.GetComponent<Display>().addedCar(false);

        }
        else if (Input.GetButtonDown(_playerInput.backButton) == true && _mouse.activeSelf == false && _ready == true)
        {
            currentVehicle = 0;
            _ready = false;
            _isInfoOn = false;
            resetInfoScreens();
            resetCars();
            _mouse.SetActive(true);
            Grid.GetComponent<Display>().addedCar(false);
            Grid.GetComponent<Display>().confirmedCar(false);

        }

        if (Input.GetButtonDown(_playerInput.selectButton) == true && _ready == true && _isInfoOn == false)
        {
            Grid.GetComponent<Display>().confirmedCar(true);
        }

        if (Input.GetAxis(_playerInput.respawn) > 0 == true && _ready == true && _isInfoOn == false)
        {
            _isInfoOn = true;
            showInfoScreen(0);
        }

        if(_isInfoOn)
        {
            if (Mathf.Abs(Input.GetAxis(_playerInput.horizontal)) >= .2)
            {
                if (_canSwap)
                {
                    int sign = (int)Mathf.Sign(Input.GetAxisRaw(_playerInput.horizontal));

                    _pageIndex += sign;
                    _pageIndex = _pageIndex % 4;
                    showInfoScreen(_pageIndex);
                    _canSwap = false;
                }
            }
            else _canSwap = true;
        }

    }
    private void FixedUpdate()
    {
        moveCursor();
    }
    private void moveCursor()
    {
        float dir = Input.GetAxis(_playerInput.horizontal);// > 0 ? 1 : -1;
        float dir2 = Input.GetAxis(_playerInput.vertical);// > 0 ? 1 : -1;
        if(Mathf.Abs(dir) <0.1)
        {
            dir = 0;
        }
        if (Mathf.Abs(dir2) < 0.1)
        {
            dir2 = 0;
        }
        if (_mouse.activeSelf)
        {
            gameObject.transform.localPosition = new Vector3(gameObject.transform.localPosition.x + dir * sens, gameObject.transform.localPosition.y + dir2 * sens);
        }

    }

    public void changeReady(bool stat)
    {
        _ready = stat;
    }
    public bool checkReady()
    {
        return _ready;
    }
    public void showInfoScreen(int screenNum)
    {
        resetInfoScreens();
        infoScreens[(currentVehicle * 4) + screenNum].SetActive(true);
    }

    private void resetInfoScreens()
    {
        foreach(GameObject screen in infoScreens)
        {
            screen.SetActive(false);
        }
    }

    public void viewCar()
    {
        resetCars();
        carModels[currentVehicle].SetActive(true);
    }

    private void resetCars()
    {
        foreach (GameObject car in carModels)
        {
            car.SetActive(false);
        }
    }
}
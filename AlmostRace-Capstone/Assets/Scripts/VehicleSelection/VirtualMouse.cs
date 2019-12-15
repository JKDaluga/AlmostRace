using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class VirtualMouse : UIBehaviour
{ 

    public int currentVehicle;

    private PlayerInput _playerInput;
    public GameObject _mouse;
    public GameObject Grid;
    private bool _ready = false;
    private bool _isInfoOn = false;

    public GameObject[] infoScreens;

    private bool overlapping = true;

    public int sens = 15;

    protected override void Start()
    {
        _playerInput = GetComponent<PlayerInput>();
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

        if (Input.GetButtonDown(_playerInput.awakeButton) == true && _mouse.activeSelf == false && _ready == false)
        { 
            _mouse.SetActive(true);
            Grid.GetComponent<Display>().changeStatus(true, _playerInput.getPlayerNum()); ;
            Grid.GetComponent<Display>().addedCar(false);
            


        }
        else if (Input.GetButtonDown(_playerInput.backButton) == true && _mouse.activeSelf == false && _ready == true)
        {
            currentVehicle = 0;
            _ready = false;
            _isInfoOn = false;
            _mouse.SetActive(true);
            Grid.GetComponent<Display>().addedCar(false);
            Grid.GetComponent<Display>().confirmedCar(false);

        }

        if (Input.GetButtonDown(_playerInput.selectButton) == true && _ready == true && _isInfoOn == false)
        {
            Grid.GetComponent<Display>().confirmedCar(true);
        }

        if (Input.GetButtonDown(_playerInput.respawn) == true && _ready == true && _isInfoOn == false)
        {
            _isInfoOn = true;
            //Grid.GetComponent<Display>().confirmedCar(true);
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
        switch(currentVehicle)
        {
            case 0:
                switch (screenNum)
                {
                    case 0:
                        break;
                    case 1:
                        break;
                    case 2:
                        break;
                    case 3:
                        break;
                    default:
                        break;
                }
                break;
            case 1:
                switch (screenNum)
                {
                    case 0:
                        break;
                    case 1:
                        break;
                    case 2:
                        break;
                    case 3:
                        break;
                    default:
                        break;
                }
                break;
            default:
                break;
        }
    }
}
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

    private bool overlapping = true;

    public int sens = 15;

    protected override void Start()
    {
        _playerInput = GetComponent<PlayerInput>();
    }
    private void Update()
    {
        moveCursor();


    }

    private void moveCursor()
    {
        int dir = Input.GetAxis(_playerInput.horizontal) > 0 ? 1 : -1;
        int dir2 = Input.GetAxis(_playerInput.vertical) > 0 ? 1 : -1;

        if (Input.GetAxis(_playerInput.vertical) >= 0.1 && _mouse.activeSelf || Input.GetAxis(_playerInput.vertical) <= -0.1 && _mouse.activeSelf)
        {
            gameObject.transform.localPosition = new Vector3(gameObject.transform.localPosition.x + 0, gameObject.transform.localPosition.y + dir2 * sens);
        }
        if (Input.GetAxis(_playerInput.horizontal) >= 0.1 && _mouse.activeSelf || Input.GetAxis(_playerInput.horizontal) <= -0.1 && _mouse.activeSelf)
        {
            gameObject.transform.localPosition = new Vector3(gameObject.transform.localPosition.x + dir * sens, gameObject.transform.localPosition.y + 0);
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
            _mouse.SetActive(true);
            Grid.GetComponent<Display>().addedCar(false);
            Grid.GetComponent<Display>().confirmedCar(false);

        }

        if (Input.GetButtonDown(_playerInput.selectButton) == true && _ready == true)
        {
            Grid.GetComponent<Display>().confirmedCar(true);
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
    
}
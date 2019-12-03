using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 Eddie B
 12/2/2019
 Allows crosshairs to be visible only to the relevant player
     */

public class CrosshairBehavior : MonoBehaviour
{
    public VehicleInput vehicleInputScript;
    // Start is called before the first frame update
    void Start()
    {
      if(vehicleInputScript != null)
        {
            int _playerNum = vehicleInputScript.getPlayerNum();
            switch(_playerNum)
            {
                case 1:
                    gameObject.transform.parent.gameObject.layer = 14; //current layer for "Player1"
                    break;
                case 2:
                    gameObject.transform.parent.gameObject.layer = 15; //current layer for "Player2"
                    break;
                case 3:
                    gameObject.transform.parent.gameObject.layer = 18; //current layer for "Player3"
                    break;
                case 4:
                    gameObject.transform.parent.gameObject.layer = 19; //current layer for "Player4"
                    break;
            }
        }
        else
        {
            Debug.Log("You forgot to give: "
                + gameObject.transform.parent.parent.parent.parent.parent.parent.name + "'s crosshair a reference!");
        }
    }

    //TODO, turn red when aiming at enemy.
}

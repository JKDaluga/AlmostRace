using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionSetTrigger : MonoBehaviour
{
    [Header("False for first gate, True for Second")]
    public bool isSecondTrigger;

    private int place, previousPlace;


    void Start()
    {
        previousPlace = -1;
        place = 0;
    }


    private void OnTriggerEnter(Collider other)
    {
        RaycastCar temp = other.GetComponent<RaycastCar>();

        if (isSecondTrigger == false )
        {
            if (temp != null)
            {
                if ((DataManager.instance.playerInfo[temp.playerID - 1].placeRace1 > previousPlace))
                {
                    DataManager.instance.playerInfo[temp.playerID - 1].placeRace1 = place;
                    print("Player " + DataManager.instance.playerInfo[temp.playerID - 1].placeRace1);
                    previousPlace++;
                    place++;
                }
            }
        }
        else if (isSecondTrigger == true)
        {
            if (temp != null)
            {
                if ((DataManager.instance.playerInfo[temp.playerID - 1].placeRace2 > previousPlace))
                {
                    DataManager.instance.playerInfo[temp.playerID - 1].placeRace2 = place;
                    previousPlace++;
                    place++;
                }
            }
        }
    }

}
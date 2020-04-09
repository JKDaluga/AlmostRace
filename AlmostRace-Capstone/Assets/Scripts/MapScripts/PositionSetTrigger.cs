using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionSetTrigger : MonoBehaviour
{
    [Header("False for first gate, True for Second")]
    public bool isSecondTrigger;

    private int place, previousPlace;

    private int numHumans, humansFinish;

    private HypeManager hm;
    private RaceManager rm;


    void Start()
    {
        previousPlace = -1;
        place = 0;
        numHumans = DataManager.instance.getNumActivePlayers();
        humansFinish = 0;
        hm = FindObjectOfType<HypeManager>();
        rm = hm.GetComponent<RaceManager>();
    }


    private void supplyTime(int index)
    {
        float fin = DataManager.instance.playerInfo[index-1].timerRace2 + Random.Range(0.5f, 7.5f);
        DataManager.instance.playerInfo[index].timerRace2 = fin;
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
                temp.finished = true;
                if ((DataManager.instance.playerInfo[temp.playerID - 1].placeRace2 > previousPlace))
                {
                    DataManager.instance.playerInfo[temp.playerID - 1].placeRace2 = place;
                    DataManager.instance.playerInfo[temp.playerID - 1].timerRace2 = rm.time;
                    previousPlace++;
                    place++;
                    if(DataManager.instance.playerInfo[temp.playerID - 1].isActive)
                    {
                        humansFinish++;
                    }
                    if(humansFinish >= numHumans)
                    {
                        for(int i = temp.playerID; i < hm.vehicleList.Count; i++)
                        {
                            supplyTime(i);
                        }
                        hm.EndGame();
                    }
                }
            }
        }
    }

}
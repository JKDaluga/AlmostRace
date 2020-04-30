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


    private float supplyTime(int index, float previousTime, bool isFirstSection)
    {
        
        float fin = previousTime + Random.Range(1f, 7.5f);
        if (isFirstSection) DataManager.instance.playerInfo[index].timerRace1 = fin;
        else DataManager.instance.playerInfo[index].timerRace2 = fin;
        return fin;
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
                    previousPlace++;
                    place++;
                }
                if (DataManager.instance.playerInfo[temp.playerID - 1].isActive)
                {
                    humansFinish++;
                }
                if (humansFinish >= numHumans)
                {
                    // give all AI that havent finshed yet a time
                    float previousTime = rm.time;
                    for (int i = 0; i < DataManager.instance.playerInfo.Length; i++)
                    {
                        //If they haven't finished the race yet
                        if (DataManager.instance.playerInfo[i].placeRace1 == 3 && DataManager.instance.playerInfo[i].timerRace1 == 0f)
                        {
                            previousTime = supplyTime(i, previousTime, true);
                            place++;
                        }
                    }
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
                        // give all AI that havent finshed yet a time
                        float previousTime = rm.time;
                        for (int i = 0; i < DataManager.instance.playerInfo.Length; i++)
                        {
                            //If they haven't finished the race yet
                            if(DataManager.instance.playerInfo[i].placeRace2 == 3 && DataManager.instance.playerInfo[i].timerRace2 == 0f)
                            {
                                previousTime = supplyTime(i, previousTime, false);
                                place++;
                            }
                        }
                        hm.EndGame();
                    }
                }
            }
        }
    }
}
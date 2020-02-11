using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleAwardsTracker : MonoBehaviour
{
    DataManager players;
    RaycastCar carInputs;


    // Start is called before the first frame update
    void Start()
    {
        players = FindObjectOfType<DataManager>();
        carInputs = FindObjectOfType<RaycastCar>();
    }

    // Update is called once per frame
    void Update()
    {

        if (carInputs.drift)
        {
            driftUpdate();
            print("Drift award amount: " + players.playerInfo[0].driftTimer);
        }

    }

    private void driftUpdate()
    {

        players.playerInfo[0].driftTimer += .1f;
        
    }
}

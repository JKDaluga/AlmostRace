using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleAwardsTracker : MonoBehaviour
{
    DataManager players;
    RaycastCar carInputs;
    VehicleAbilityBehavior abilityInputs;
    VehicleInput playerNum;

    public float offensiveAwardTimer = 0f;
    public float defenseAwardTimer = 0f;
    public float boostAwardTimer = 0f;

    // Start is called before the first frame update
    void Start()
    {
        players = FindObjectOfType<DataManager>();
        carInputs = FindObjectOfType<RaycastCar>();
        abilityInputs = FindObjectOfType<VehicleAbilityBehavior>();
        playerNum = FindObjectOfType<VehicleInput>();
        
    }

    // Update is called once per frame
    void Update()
    {

        if (carInputs.drift)
        {
            driftUpdate();
        }

    }

    public void awardUpdate(char abilityType, int playerNumber)
    {
        if (abilityType == 'o')
        {
            players.playerInfo[playerNumber - 1].offensiveAbilityUsed += 1;
        }
        else if(abilityType=='d')
        {
            players.playerInfo[playerNumber - 1].defenseAbilityUsed += 1;
        }
        else
        {
            players.playerInfo[playerNumber - 1].boostAbilityUsed += 1;
        }
    }

    private void driftUpdate()
    {

        players.playerInfo[playerNum.getPlayerNum() - 1].driftTimer += .1f;
        
    }
}

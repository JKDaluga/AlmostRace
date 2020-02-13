using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleAwardsTracker : MonoBehaviour
{
    DataManager players;
    RaycastCar carInputs;
    VehicleAbilityBehavior abilityInputs;

    public float offensiveAwardTimer = 0f;

    // Start is called before the first frame update
    void Start()
    {
        players = FindObjectOfType<DataManager>();
        carInputs = FindObjectOfType<RaycastCar>();
        abilityInputs = FindObjectOfType<VehicleAbilityBehavior>();
    }

    // Update is called once per frame
    void Update()
    {

        if (abilityInputs.offensiveAbilityDark == true && offensiveAwardTimer <= 0f)
        {
            players.playerInfo[0].offensiveAbilityUsed += 1;
            print("Offensive ability used: " + players.playerInfo[0].offensiveAbilityUsed);
            offensiveAwardTimer = abilityInputs.offensiveAbilityRecharge;
        }

        if (carInputs.drift)
        {
            driftUpdate();
        }


        offensiveAwardTimer -= Time.deltaTime;
    }

    private void driftUpdate()
    {

        players.playerInfo[0].driftTimer += .1f;
        
    }
}

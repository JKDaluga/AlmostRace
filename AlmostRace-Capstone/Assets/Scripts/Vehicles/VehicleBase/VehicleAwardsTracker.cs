using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleAwardsTracker : MonoBehaviour
{
    DataManager players;
    RaycastCar carInputs;
    VehicleAbilityBehavior abilityInputs;

    public float offensiveAwardTimer = 0f;
    public float defenseAwardTimer = 0f;
    public float boostAwardTimer = 0f;

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
            offenseUpdate();
        }

        if (abilityInputs.defensiveAbilityDark == true && defenseAwardTimer <= 0f)
        {
            defenseUpdate();
        }

        if (abilityInputs.boostAbilityDark == true && boostAwardTimer <= 0f)
        {
            boostUpdate();
        }

        if (carInputs.drift)
        {
            driftUpdate();
        }


        offensiveAwardTimer -= Time.deltaTime;
    }

    private void offenseUpdate()
    {
        players.playerInfo[0].offensiveAbilityUsed += 1;
        print("Offensive ability used: " + players.playerInfo[0].offensiveAbilityUsed);
        offensiveAwardTimer = abilityInputs.offensiveAbilityRecharge;
    }

    private void defenseUpdate()
    {
        players.playerInfo[0].defenseAbilityUsed += 1;
        print("Defensive ability used: " + players.playerInfo[0].defenseAbilityUsed);
        defenseAwardTimer = abilityInputs.defensiveAbilityRecharge;
    }

    private void boostUpdate()
    {
        players.playerInfo[0].boostAbilityUsed += 1;
        print("Boost ability used: " + players.playerInfo[0].boostAbilityUsed);
        boostAwardTimer = abilityInputs.boostAbilityRecharge;
    }

    private void driftUpdate()
    {

        players.playerInfo[0].driftTimer += .1f;
        
    }
}

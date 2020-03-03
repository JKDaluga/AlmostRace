using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    Author: Jason Daluga
    Purpose: PlayerInfo is a storage class for player data
    Data Manager is what we use to player car data for race manager
*/

[System.Serializable]
public class PlayerInfo
{
    public bool isActive = false;
    public int playerID = 0;
    public int carID = 0;
    public int controllerID = 0;
    public int boostAbilityUsed = 0;
    public int offensiveAbilityUsed = 0;
    public int defenseAbilityUsed = 0;
    public float driftTimer = 0;
}

public class DataManager : MonoBehaviour
{
    public static DataManager instance;
    public PlayerInfo[] playerInfo;

    private void Awake()
    {
        if(instance != null && this != instance)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    public int getNumActivePlayers()
    {
        int count = 0;
        foreach(PlayerInfo player in playerInfo)
        {
            if (player.isActive) count++;
        }
        return count;
    }

    public void resetData()
    {
        playerInfo = new PlayerInfo[4];
        for (int i = 0; i < playerInfo.Length; i++)
        {
            playerInfo[i] = new PlayerInfo();
            playerInfo[i].playerID = i + 1;
        }
    }
}

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
    public float hypeAmount = 0f;
    public int controllerID = 0;

    //stats used for awards
    public int boostAbilityUsed = 0;
    public int offensiveAbilityUsed = 0;
    public int defenseAbilityUsed = 0;
    public float driftTimer = 0;
    public float numKills = 0f;
    public float numDeaths = 0f;
    public int placeRace1 = 0;
    public float timerRace1 = 0;
    public int placeRace2 = 0;
    public float timerRace2 = 0;
}

public class DataManager : MonoBehaviour
{
    public static DataManager instance;
    public PlayerInfo[] playerInfo;
    public Dictionary<int, PlayerUIManager> playerUIDictionary = new Dictionary<int, PlayerUIManager>();

    private bool spawnAI;

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

    public string convertTime(float time)
    {
        float minutes = Mathf.Floor(time / 60);
        float seconds = time % 60;
        float milliseconds = (time * 1000) % 1000;


        string timeText = minutes.ToString("00") + ":" + seconds.ToString("00") + ":" + milliseconds.ToString("000");

        return timeText;
    }

    public string convertTimeSeconds(float time)
    {
        float minutes = Mathf.Floor(time / 60);
        float seconds = time % 60;


        string timeText = minutes.ToString("00") + ":" + seconds.ToString("00");

        return timeText;
    }

    public bool CheckAISpawning()
    {
        return spawnAI;
    }
    public void ChangeAISpawn(bool value)
    {
        spawnAI = value;
    }

    public void ResetData()
    {
        playerUIDictionary.Clear();
        for (int i = 0; i < playerInfo.Length; i++)
        {
            playerInfo[i].isActive = false;
            playerInfo[i].playerID = i + 1;
            playerInfo[i].controllerID = 0;
            playerInfo[i].hypeAmount = 0f;
            playerInfo[i].placeRace1 = playerInfo.Length - 1;
            playerInfo[i].placeRace2 = playerInfo.Length - 1;
            playerInfo[i].boostAbilityUsed = 0;
            playerInfo[i].offensiveAbilityUsed = 0;
            playerInfo[i].defenseAbilityUsed = 0;
            playerInfo[i].numKills = 0f;
            playerInfo[i].numDeaths = 0f;
        }
    }

    public void TriggerKillUI(int playerID)
    {
      
        if(playerUIDictionary.ContainsKey(playerID))
        {
            playerUIDictionary[playerID].TriggerKillPopup();
        }
        else
        {
          //  Debug.Log("playerNumber @: " + playerID + " was not found!!");
        }
    }
}
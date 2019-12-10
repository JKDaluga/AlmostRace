using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * 
 * 
 * 
 */

[System.Serializable]
public class PlayerInfo
{
    public bool isActive = false;
    public int carID = 0;
    public int colorID = 0;
}

public class DataManager : MonoBehaviour
{
    public static DataManager instance;
    public PlayerInfo[] playerInfo;

    private void Awake()
    {
        if(instance != null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
            //playerInfo = new PlayerInfo[4];
            //for(int i = 0; i < playerInfo.Length; i++)
            //{
            //    playerInfo[i] = new PlayerInfo();
            //}
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
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    private static DataManager instance;

    public class PlayerInfo
    {
        public bool isActive = false;
        public int carID = 0;
        public int colorID = 0;
    }

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
            playerInfo = new PlayerInfo[4];
            for(int i = 0; i < playerInfo.Length; i++)
            {
                playerInfo[i] = new PlayerInfo();
            }
        }
    }
}

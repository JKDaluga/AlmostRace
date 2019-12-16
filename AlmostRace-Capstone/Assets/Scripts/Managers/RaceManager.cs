using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaceManager : MonoBehaviour
{
    private DataManager dm;

    public GameObject[] carPool;

    public Transform[] spawnLocations;

    private Rect ratio;
    public float twoPlayerUIScalingFactor = 1;
    public float threePlusPlayerUIScalingFactor = 1;

    private GameObject eventPanel;
    private RectTransform rt;

    // Start is called before the first frame update
    void Start()
    {
        eventPanel = GameObject.FindGameObjectWithTag("EventPanel");
        rt = eventPanel.GetComponent<RectTransform>();
        rt.offsetMin = new Vector2(0, 0);
        rt.offsetMax = new Vector2(0, 0);

        dm = DataManager.instance;
        if(dm == null)
        {
            Debug.LogError("Cannot find DataManager");
        }
        else
        {
            int playerCount = dm.getNumActivePlayers();
            int playerNum = 1;

            foreach (PlayerInfo player in dm.playerInfo)
            {
                if(player.isActive)
                {
                    spawnPlayer(player, playerCount, playerNum);
                    playerNum++;
                }
            }
        }
        eventPanel.SetActive(false);
    }

    void spawnPlayer(PlayerInfo player, int playerCount, int playerNum)
    {
        // spawns the car on the map in the right spot
        //print(player.carID * 4 + player.playerID - 1);
        GameObject car = Instantiate(carPool[player.carID * 4 + player.playerID - 1], spawnLocations[player.carID].position, spawnLocations[player.carID].rotation);

        SphereCarController sphereCar = car.GetComponentInChildren<SphereCarController>();
        VehicleInput v = car.GetComponentInChildren<VehicleInput>();
        v.setPlayerNum(playerNum);
        if (sphereCar.tiltShift.gameObject != null)
        {
            Camera c = sphereCar.tiltShift.gameObject.GetComponent<Camera>();
            if (playerCount == 2)
            {
                setUp_2(player.playerID, c);
            }
            else if (playerCount > 2)
            {
                setUp_3_4(player.playerID, c);
            }
        }

        setUpCarUI(sphereCar, player.playerID, playerCount);

        if(playerCount == 1)
        {
            rt.offsetMin = new Vector2(0, 150);
            rt.offsetMax = new Vector2(0, 150);
        }
    }

    void setUpCarUI(SphereCarController car, int playerID, int playerCount)
    {
        // Setup car dependent on # of players and car #
        if(playerCount == 2)
        {
            if(playerID == 2)
            {
                car.UIPanel.anchorMin = new Vector2(0, 0);
                car.UIPanel.anchorMax = new Vector2(1, .5f);
            }
            else
            {
                car.UIPanel.anchorMin = new Vector2(0, .5f);
                car.UIPanel.anchorMax = new Vector2(1, 1);
            }
            for (int i = 0; i < car.secondaryUIPanel.childCount; i++)
            {
                car.secondaryUIPanel.GetChild(i).localScale = car.secondaryUIPanel.GetChild(i).localScale * twoPlayerUIScalingFactor;
            }
        }
        else if(playerCount >= 3)
        {
            if (playerID == 1)
            {
                car.UIPanel.anchorMin = new Vector2(0, .5f);
                car.UIPanel.anchorMax = new Vector2(.5f, 1);
                car.UIPanel.pivot = new Vector2(1, 1);
            }
            else if (playerID == 2)
            {
                car.UIPanel.anchorMin = new Vector2(.5f, .5f);
                car.UIPanel.anchorMax = new Vector2(1, 1);
                car.UIPanel.pivot = new Vector2(1, 1);
            }
            else if (playerID == 3)
            {
                car.UIPanel.anchorMin = new Vector2(0, 0);
                car.UIPanel.anchorMax = new Vector2(.5f, .5f);
                car.UIPanel.pivot = new Vector2(0, 0);
            }
            else if (playerID == 4)
            {
                car.UIPanel.anchorMin = new Vector2(.5f, 0);
                car.UIPanel.anchorMax = new Vector2(1, .5f);
                car.UIPanel.pivot = new Vector2(1, 0);
            }
            for (int i = 0; i < car.secondaryUIPanel.childCount; i++)
            {
                car.secondaryUIPanel.GetChild(i).localScale = car.secondaryUIPanel.GetChild(i).localScale * threePlusPlayerUIScalingFactor;
            }
        }
    }

    //Camera Scaling Code by Leo
    private void setUp_2(int num, Camera c)
    {
        if (num == 1)
        {
            ratio = c.rect;
            ratio.width = 1f;
            ratio.height = 0.5f;
            ratio.y = 0.5f;
            ratio.x = 0f;
            c.rect = ratio;
        }
        else if (num == 2)
        {
            ratio = c.rect;
            ratio.width = 1f;
            ratio.height = 0.5f;
            ratio.x = 0f;
            ratio.y = 0f;
            c.rect = ratio;
        }
    }
    private void setUp_3_4(int num, Camera c)
    {
        if (num == 1)
        {
            ratio = c.rect;
            ratio.width = 0.5f;
            ratio.height = 0.5f;
            ratio.y = 0.5f;
            ratio.x = 0f;
            c.rect = ratio;
        }
        else if (num == 2)
        {
            ratio = c.rect;
            ratio.width = 0.5f;
            ratio.height = 0.5f;
            ratio.x = 0.5f;
            ratio.y = 0.5f;
            c.rect = ratio;
        }
        else if (num == 3)
        {
            ratio = c.rect;
            ratio.width = 0.5f;
            ratio.height = 0.5f;
            ratio.x = 0f;
            ratio.y = 0f;
            c.rect = ratio;
        }
        else if (num == 4)
        {
            ratio = c.rect;
            ratio.width = 0.5f;
            ratio.height = 0.5f;
            ratio.x = 0.5f;
            ratio.y = 0f;
            c.rect = ratio;
        }
    }
}

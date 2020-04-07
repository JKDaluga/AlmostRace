using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    Author: Jason Daluga
    Purpose: Sets up the race based on the data provide from datamanager
*/
public class RaceManager : MonoBehaviour
{
    private DataManager dm;
    public GameObject[] orderedSplines;
    public bool spawnAI = false;
    public GameObject[] carPool;

    public GameObject[] AICar;
    private int AIindex;

    public Transform[] spawnLocations;

    private Rect ratio;
    public float twoPlayerUIScalingFactor = 1;
    public float threePlusPlayerUIScalingFactor = 1;

    private GameObject eventPanel;
    private RectTransform rt;
    public GameObject fourthPlayerPanel;

    public RaycastCar[] cars;

    public bool inArena;

    private bool inCountDown = true;

    private SplineSwapTrigger aiMan;

    // Start is called before the first frame update
    void Start()
    {
        aiMan = FindObjectOfType<SplineSwapTrigger>();
        aiMan.orderedSplines = orderedSplines;
        fourthPlayerPanel.SetActive(false);
        eventPanel = GameObject.FindGameObjectWithTag("EventPanel");
        AIindex = Random.Range(0, AICar.Length);
        rt = eventPanel.GetComponent<RectTransform>();
        rt.offsetMin = new Vector2(0, 0);
        rt.offsetMax = new Vector2(0, 0);
        time = 0;
        
        dm = DataManager.instance;
        if(dm == null)
        {
            Debug.LogError("Cannot find DataManager");
        }
        else
        {
            int playerCount = dm.getNumActivePlayers();
            if (spawnAI)
            {
                cars = new RaycastCar[dm.playerInfo.Length];
            }
            else
            {
                cars = new RaycastCar[playerCount];
            }
            int playerNum = 1;
            int AINum = 0;

            foreach (PlayerInfo player in dm.playerInfo)
            {
                if(player.isActive)
                {
                    spawnPlayer(player, playerCount);
                    playerNum++;
                }
                else
                {
                    if (spawnAI == true)
                    {
                        RaycastCar aiCar = Instantiate(AICar[AIindex], spawnLocations[playerNum + AINum - 1].position, spawnLocations[playerNum + AINum - 1].rotation).GetComponentInChildren<RaycastCar>();
                        cars[playerNum + AINum - 1] = aiCar;
                        aiMan.aiCars.Add(aiCar);
                        aiCar.playerID = player.playerID;
                        AINum++;
                        AIindex++;
                        if(AIindex >= AICar.Length)
                        {
                            AIindex = 0;
                        }
                    }
                }
            }
            
        }
        eventPanel.SetActive(false);
        aiMan.updateAI();
    }

    public void SetCountDown()
    {
        inCountDown = false;
    }

    public float time;
    public string timeText;

    private void Update()
    {
        if (inCountDown == false && !inArena)
        {
            time += Time.deltaTime;
            timeText = DataManager.instance.convertTime(time);
        }

    }

    void spawnPlayer(PlayerInfo player, int playerCount)
    {
        // spawns the car on the map in the right spot
        GameObject car = Instantiate(carPool[player.carID * 4 + player.playerID - 1], spawnLocations[player.playerID - 1].position, spawnLocations[player.playerID - 1].rotation);
        RaycastCar raycastCar = car.GetComponentInChildren<RaycastCar>();
        cars[player.playerID - 1] = raycastCar;
        raycastCar.playerID = player.playerID;
        VehicleInput v = car.GetComponentInChildren<VehicleInput>();
        v.setPlayerNum(player.controllerID);
        if (raycastCar != null)
        {
            Camera c = raycastCar.gameObject.transform.parent.GetComponentInChildren<Camera>();
            if (playerCount == 2)
            {
                setUp_2(player.playerID, c);
            }
            else if (playerCount > 2)
            {
                setUp_3_4(player.playerID, c);
            }
        }

        setUpCarUI(raycastCar, player.playerID, playerCount);

        if(playerCount == 1)
        {
            rt.offsetMin = new Vector2(0, 150);
            rt.offsetMax = new Vector2(0, 150);
        }
    }

    void setUpCarUI(RaycastCar car, int playerID, int playerCount)
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
            if(playerCount == 3)
            {
                fourthPlayerPanel.SetActive(true);
            }
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

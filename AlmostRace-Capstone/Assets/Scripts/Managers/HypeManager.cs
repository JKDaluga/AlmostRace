using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using TMPro;

/*
Author: Jake Velicer
Purpose: Stores each vehicle object in a list,
called to keep that list ordered by the vehicles with the most hype,
updates the UI accordingly.

Edited 11/10/2019: Eddie B
Added a coroutine to track the total hype of the players. Needed for the HypeGates.
Made sense to have the code happen once here as opposed to 4+ times across the board.

Edited 3/4/2020: Robyn R
Adjusted script to account for the new ways we'll be tracking score.

Edited 3/5/2020: Jason D
Refactoring for our simplified game loop
*/

[System.Serializable]
public class WinScreenBox
{
    public Image background;
    public TextMeshProUGUI playerTag;
    public GameObject ScoreHeader;
    public TextMeshProUGUI hypeAmount;
    public TextMeshProUGUI finishTime;
    public TextMeshProUGUI finishHype;
    public TextMeshProUGUI finishTime2;
    public TextMeshProUGUI finishHype2;
    public TextMeshProUGUI numKills;
    public TextMeshProUGUI killHype;
}

public class HypeManager : MonoBehaviour
{
    public List<PlayerInfo> vehicleList = new List<PlayerInfo>();
    public float totalHype;
    public Text winnerText;
    private float tempTotal;
    public GameObject countdownObj;
    public float countdownLength = 45.0f;
    public GameObject eventPanel;
    public GameObject winScreen;
    public Image bottomLineLogo;
    public bool isGameEnded = false;

    float humansFinished;

    public WinScreenBox[] winScreenBoxes;
    public string[] awards;
    public string[] awardsNumbers;
    public Color[] playerColors;
    //amount of time between when end screen pops up and the player is allowed to press any button to continue
    public float inputDelayMax = 3f;
    private float inputDelay;
    private DataManager dm;
    private RaceManager rm;

    [Header("Hype Value Variables")]
    public float hypePerKill;
    public float[] hypeForRacePosition;
    public float hypeLosePerDeath;

    int numNodes;

    // Start is called before the first frame update
    void Start()
    {
        inputDelay = 0;
        awards = new string[4];
        awardsNumbers = new string[4];
        rm = GetComponent<RaceManager>();
        dm = DataManager.instance;
        if (dm == null)
        {
            Debug.LogError("Cannot find DataManager");
        }
    }

    public void setNumNodes(int n)
    {
        numNodes = n;
    }

    public IEnumerator EndGameCountDown(float timerTime)
    {
        float tempTime = timerTime;
        countdownObj.SetActive(true);
        TextMeshProUGUI countdownText = countdownObj.GetComponent<TextMeshProUGUI>();
        while (tempTime > 0)
        {
            countdownText.text = "" + tempTime.ToString("F0");
            tempTime -= Time.deltaTime;

            if(humansFinished >= dm.getNumActivePlayers())
            {
                tempTime = 0;
            }

            yield return null;
        }
        countdownObj.SetActive(false);
        if (humansFinished < dm.getNumActivePlayers())
        {
            List<int> places = new List<int>();

            foreach(RaycastCar i in rm.cars)
            {
                if (!i.finished)
                {
                    dm.playerInfo[i.playerID - 1].timerRace2 = rm.time;
                }

                places.Add(numNodes - i.closestIndex);
            }
            places.Sort();
            
            for(int i = 0; i < places.Count; i++)
            {
                if (!rm.cars[i].finished)
                {
                    dm.playerInfo[rm.cars[i].playerID - 1].placeRace2 = i;
                }
            }

            EndGame();
        }
    }

    // Sorts the list based upon which game object has the most hype in their VehicleHypeBehavior script
    public void HypeListSort()
    {
        vehicleList.Sort(
            delegate(PlayerInfo p1, PlayerInfo p2)
            {
                return p1.hypeAmount.CompareTo(p2.hypeAmount);
            }
        );
        // Put in descending order
        vehicleList.Reverse();
    }

    public void HypeListAssign()
    {
        foreach(PlayerInfo player in dm.playerInfo)
        {
            if(player.isActive || DataManager.instance.CheckAISpawning())
            {
                vehicleList.Add(player);
            }
        }
    }

    public void EndGame()
    {
        if(!isGameEnded)
        {
            isGameEnded = true;
            MenuController.setIsGamePaused(true);
            eventPanel.SetActive(true);
            calculateHype();
            HypeListAssign();
            HypeListSort();
            populateWinScreen();
            winScreen.SetActive(true);

            Invoke("DisableEvents", 3);

            AudioManager.instance.PlayWithoutSpatial("Victory Music");
        }
    }

    private void Update()
    {
        if(isGameEnded)
        {
            if(inputDelay >= inputDelayMax)
            {
                if (Input.anyKeyDown) ReturnToMainMenu();
            }
            else
            {
                inputDelay += Time.deltaTime;
            }
        }
    }

    private void ReturnToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
        Time.timeScale = 1f;
        winScreen.gameObject.SetActive(false);
    }

    public void DisableEvents()
    {
        eventPanel.SetActive(false);
    }

    public void calculateHype()
    {
        foreach(PlayerInfo player in DataManager.instance.playerInfo)
        {
            player.hypeAmount = (player.numKills * hypePerKill) + hypeForRacePosition[player.placeRace1] + hypeForRacePosition[player.placeRace2] - (player.numDeaths * hypeLosePerDeath);
        }
    }

    public void incrementHumansFinished()
    {
        humansFinished++;
    }

    public void populateWinScreen()
    {
        for (int i = 0; i < winScreenBoxes.Length; i++)
        {
            if (i < vehicleList.Count)
            {
                int playerNum = vehicleList[i].playerID;
                if(i == 0)
                {
                    bottomLineLogo.color = playerColors[playerNum - 1];
                }
                winScreenBoxes[i].background.color = playerColors[playerNum - 1];
                if (vehicleList[i].isActive)
                {
                    winScreenBoxes[i].playerTag.text = "Player " + playerNum;
                }
                else
                {
                    winScreenBoxes[i].playerTag.text = "BOT";
                }
                winScreenBoxes[i].ScoreHeader.SetActive(true);
                winScreenBoxes[i].hypeAmount.text = vehicleList[i].hypeAmount.ToString();
                winScreenBoxes[i].finishTime.text = "Area 1 - " + DataManager.instance.convertTimeSeconds(vehicleList[i].timerRace1);
                winScreenBoxes[i].finishHype.text = hypeForRacePosition[vehicleList[i].placeRace1].ToString();
                winScreenBoxes[i].numKills.text = "Kills - " + vehicleList[i].numKills.ToString();
                winScreenBoxes[i].killHype.text = (vehicleList[i].numKills * hypePerKill).ToString();
                winScreenBoxes[i].finishTime2.text = "Area 2 - " + DataManager.instance.convertTimeSeconds(vehicleList[i].timerRace2);
                winScreenBoxes[i].finishHype2.text = hypeForRacePosition[vehicleList[i].placeRace2].ToString();
            }
        }
    }
}

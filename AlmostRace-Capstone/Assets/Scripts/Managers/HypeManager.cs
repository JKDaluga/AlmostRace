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
    public TextMeshProUGUI hypeAmount;
    public TextMeshProUGUI finishTime;
    public TextMeshProUGUI finishHype;
    public TextMeshProUGUI finishTime2;
    public TextMeshProUGUI finishHype2;
    public TextMeshProUGUI numKills;
    public TextMeshProUGUI killHype;
    public TextMeshProUGUI Awards;
}

public class HypeManager : MonoBehaviour
{
    public List<PlayerInfo> vehicleList = new List<PlayerInfo>();
    public float totalHype;
    public Text winnerText;
    private float tempTotal;
    public GameObject countdownObj;
    public GameObject eventPanel;
    public GameObject winScreen;
    public bool isGameEnded = false;

    public WinScreenBox[] winScreenBoxes;
    public string[] awards;
    public Color[] playerColors;
    //amount of time between when end screen pops up and the player is allowed to press any button to continue
    public float inputDelayMax = 3f;
    private float inputDelay;
    private DataManager dm;

    [Header("Hype Value Variables")]
    public float hypePerKill;
    public float[] hypeForRacePosition;
    public float hypeLosePerDeath;

    // Start is called before the first frame update
    void Start()
    {
        inputDelay = 0;
        awards = new string[4];
        dm = DataManager.instance;
        if (dm == null)
        {
            Debug.LogError("Cannot find DataManager");
        }
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
            yield return null;
        }
        countdownObj.SetActive(false);
        EndGame();
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
            vehicleList.Add(player);
        }
    }

    public void EndGame()
    {
        isGameEnded = true;
        eventPanel.SetActive(true);
        calculateHype();
        FindObjectOfType<WinScreen>().chooseWinners();
        HypeListAssign();
        HypeListSort();
        populateWinScreen();
        winScreen.SetActive(true);

        Invoke("DisableEvents", 3);

        AudioManager.instance.PlayWithoutSpatial("Victory Music");
    }

    private void Update()
    {
        if(isGameEnded)
        {
            if(inputDelay <= inputDelayMax)
            {
                if (Input.anyKeyDown) ReturnToMainMenu();
            }
            else
            {
                inputDelay += Time.deltaTime;
            }
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            EndGame();
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

    public void populateWinScreen()
    {
        for (int i = 0; i < winScreenBoxes.Length; i++)
        {
            if (i < vehicleList.Count)
            {
                int playerNum = vehicleList[i].playerID;
                winScreenBoxes[i].background.color = playerColors[playerNum - 1];
                if (vehicleList[i].isActive)
                {
                    winScreenBoxes[i].playerTag.text = "Player " + playerNum;
                }
                else
                {
                    winScreenBoxes[i].playerTag.text = "BOT";
                }
                winScreenBoxes[i].hypeAmount.text = vehicleList[i].hypeAmount.ToString();
                winScreenBoxes[i].finishTime.text += DataManager.instance.convertTimeSeconds(vehicleList[i].timerRace1);
                winScreenBoxes[i].finishHype.text = hypeForRacePosition[vehicleList[i].placeRace1].ToString();
                winScreenBoxes[i].numKills.text += vehicleList[i].numKills.ToString();
                winScreenBoxes[i].killHype.text = (vehicleList[i].numKills * hypePerKill).ToString();
                winScreenBoxes[i].finishTime2.text += DataManager.instance.convertTimeSeconds(vehicleList[i].timerRace2);
                winScreenBoxes[i].finishHype2.text = hypeForRacePosition[vehicleList[i].placeRace2].ToString();
                winScreenBoxes[i].Awards.text = (vehicleList[i].hypeAmount - ((vehicleList[i].numKills * hypePerKill) + hypeForRacePosition[vehicleList[i].placeRace1] + hypeForRacePosition[vehicleList[i].placeRace2] - (vehicleList[i].numDeaths * hypeLosePerDeath))).ToString() ;
            }
            else
            {
                winScreenBoxes[i].background.gameObject.SetActive(false);
            }
        }
    }
}

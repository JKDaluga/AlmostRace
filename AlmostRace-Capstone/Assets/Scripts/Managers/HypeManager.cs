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
    public TextMeshProUGUI numKills;
    public TextMeshProUGUI Awards;
}

public class HypeManager : MonoBehaviour
{
    public List<GameObject> vehicleList = new List<GameObject>();
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

    [Header(" Variables")]
    public float hypePerKill;
    public float[] hypeForRacePosition;
    public float hypeLosePerDeath;

    // Start is called before the first frame update
    void Start()
    {
        inputDelay = 0;
        awards = new string[4];
        StartCoroutine(TrackTotalHype());
    }

    public IEnumerator TrackTotalHype()
    {
        while(true)
        {
            tempTotal = 0;
            foreach (GameObject vehicle in vehicleList)
            {
                tempTotal += vehicle.GetComponent<VehicleHypeBehavior>().GetHypeAmount();
            }

            totalHype = tempTotal;

            yield return null;
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

    // Called to add the given vehicle to the vehicle list
    public void VehicleAssign(GameObject player)
    {
        vehicleList.Add(player);
    }

    // Sorts the list based upon which game object has the most hype in their VehicleHypeBehavior script
    public void VehicleSort()
    {
        vehicleList.Sort(
            delegate(GameObject p1, GameObject p2)
            {
                return p1.GetComponent<VehicleHypeBehavior>().GetHypeAmount()
                .CompareTo(p2.GetComponent<VehicleHypeBehavior>().GetHypeAmount());
            }
        );
        // Put in descending order
        vehicleList.Reverse();
    }

    // Sorts the list at the beginning of the game based on player number rather than hype amount
    private void BeginningVehicleSort()
    {
        vehicleList.Sort(
            delegate(GameObject p1, GameObject p2)
            {
                return p1.GetComponent<VehicleInput>().playerNumber
                .CompareTo(p2.GetComponent<VehicleInput>().playerNumber);
            }
        );
    }

    public void EndGame()
    {
        isGameEnded = true;
        float highestHype = 0f;
        GameObject winner = this.gameObject;
        foreach (GameObject entry in vehicleList)
        {
            if (entry.GetComponent<VehicleHypeBehavior>().GetHypeAmount() > highestHype)
            {
                highestHype = entry.GetComponent<VehicleHypeBehavior>().GetHypeAmount();
                winner = entry;
            }
        }
        if(winner != this.gameObject)
        {
            eventPanel.SetActive(true);
            calculateHype();
            FindObjectOfType<WinScreen>().chooseWinners();
            VehicleSort();
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
            if(inputDelay <= inputDelayMax)
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
        DataManager dataManager = FindObjectOfType<DataManager>();
        
    }

    public void populateWinScreen()
    {
        for(int i = 0; i < winScreenBoxes.Length; i++)
        {
            if (i < vehicleList.Count && vehicleList[i].GetComponent<VehicleInput>() != null)
            {
                int playerNum = vehicleList[i].GetComponent<VehicleInput>().getPlayerNum();
                winScreenBoxes[i].background.color = playerColors[playerNum - 1];
                winScreenBoxes[i].playerTag.text = "Player " + playerNum;
                winScreenBoxes[i].hypeAmount.text = vehicleList[i].GetComponent<VehicleHypeBehavior>().GetHypeAmount().ToString();
                winScreenBoxes[i].finishTime.text = "0"; //Fill with Player's Time later
                winScreenBoxes[i].numKills.text = "0"; //Fill with Player's kill count later
                winScreenBoxes[i].Awards.text += awards[playerNum - 1];
            }
            else
            {
                winScreenBoxes[i].background.gameObject.SetActive(false);
            }
        }
    }
}

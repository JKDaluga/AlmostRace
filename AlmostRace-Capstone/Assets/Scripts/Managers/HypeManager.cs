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
    */

[System.Serializable]
public class WinScreenBox
{
    public Image background;
    public TextMeshProUGUI playerTag;
    public TextMeshProUGUI hypeAmount;
    public TextMeshProUGUI Awards;
}

public class HypeManager : MonoBehaviour
{
    public List<GameObject> vehicleList = new List<GameObject>();
    private Text[] _hypeAmountDisplay;
    public float totalHype;
    //public float maxHype; //Essentially a win condition
    public Text winnerText;
    private float tempTotal;
    public GameObject countdownObj;
    public GameObject eventPanel;
    public GameObject winScreen;
    public bool isGameEnded = false;

    public WinScreenBox[] winScreenBoxes;
    public string[] awards;
    public Color[] playerColors;
    

    private void Awake()
    {
        SetUpDisplay();
    }

    // Start is called before the first frame update
    void Start()
    {
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
            //Debug.Log("tempTotal: " + tempTotal);
            totalHype = tempTotal;
            //Debug.Log("totalHype:" + totalHype);
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

    private void SetUpDisplay()
    {
        _hypeAmountDisplay = new Text[vehicleList.Count];
        for (int i = 0; i < _hypeAmountDisplay.Length; i++)
        {
            _hypeAmountDisplay[i] = GameObject.Find("HypeDisplay" + (i + 1)).GetComponent<Text>();
        }
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
        //checkWinCondition(); Doesn't need to be here, since we are moving away from the hype limit. - Eddie
      //  UIupdate();
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

        //UIupdate();
    }

    private void UIupdate()
    {
        int i = 0;
        foreach(GameObject entry in vehicleList)
        {
            _hypeAmountDisplay[i].text = entry.name.ToString() + ": " +
           // _hypeAmountDisplay[i].text = "Hype: " +
            entry.GetComponent<VehicleHypeBehavior>().GetHypeAmount().ToString();
            i++;
        }
    }

    private void checkWinCondition()
    {
        foreach(GameObject entry in vehicleList)
        {
           // if(entry.GetComponent<VehicleHypeBehavior>().GetHypeAmount() >= maxHype)
           // {
            //    Time.timeScale = 0.0f;
           //     winnerText.text = "PLAYER " + entry.GetComponent<VehicleInput>().playerNumber + " WINS!";
            //    winnerText.gameObject.SetActive(true);
          //  }
        }
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
            
            FindObjectOfType<WinScreen>().chooseWinners();
            VehicleSort();
            populateWinScreen();
            winScreen.SetActive(true);

            Invoke("DisableEvents", 3);

            AudioManager.instance.Play("Victory Music");
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

    public void populateWinScreen()
    {
        for(int i = 0; i < winScreenBoxes.Length; i++)
        {
            if (i < vehicleList.Count)
            {
                int playerNum = vehicleList[i].GetComponent<VehicleInput>().getPlayerNum();
                winScreenBoxes[i].background.color = playerColors[playerNum - 1];
                winScreenBoxes[i].playerTag.text = "Player " + playerNum;
                winScreenBoxes[i].hypeAmount.text = "Hype: " + vehicleList[i].GetComponent<VehicleHypeBehavior>().GetHypeAmount();
                winScreenBoxes[i].Awards.text += awards[playerNum - 1];
            }
            else
            {
                winScreenBoxes[i].background.gameObject.SetActive(false);
            }
        }
    }
}

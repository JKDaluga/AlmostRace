using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

    /*
    Author: Jake Velicer
    Purpose: Stores each vehicle object in a list,
    called to keep that list ordered by the vehicles with the most hype,
    updates the UI accordingly.
    */
    
public class HypeManager : MonoBehaviour
{

    public static HypeManager instance;

    public List<GameObject> vehicleList = new List<GameObject>();
    private Text[] _hypeAmountDisplay;
    public float totalHype;
    public float maxHype; //Essentially a win condition
    public Text winnerText;
    private float tempTotal;

    private void Awake()
    {
        if(instance == null) instance = this;
        else
        {
            instance.vehicleList.Clear();
            instance.SetUpDisplay();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(TrackTotalHype());
    }

    public IEnumerator TrackTotalHype()
    {
        tempTotal = 0;
        foreach(GameObject vehicle in vehicleList)
        {
            tempTotal += vehicle.GetComponent<VehicleHypeBehavior>().GetHypeAmount();
        }
        totalHype = tempTotal;
        yield return null;
    }

    private void SetUpDisplay()
    {
        instance._hypeAmountDisplay = new Text[instance.vehicleList.Count];
        for (int i = 0; i < instance._hypeAmountDisplay.Length; i++)
        {
            instance._hypeAmountDisplay[i] = GameObject.Find("HypeDisplay" + (i + 1)).GetComponent<Text>();
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
        checkWinCondition();
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
            if(entry.GetComponent<VehicleHypeBehavior>().GetHypeAmount() >= maxHype)
            {
                Time.timeScale = 0.0f;
                winnerText.text = "PLAYER " + entry.GetComponent<VehicleInput>().playerNumber + " WINS!";
                winnerText.gameObject.SetActive(true);
            }
        }
    }

}

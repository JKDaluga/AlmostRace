﻿using System.Collections;
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

    public static HypeManager HM;

    public List<GameObject> _vehicleList = new List<GameObject>();
    private Text[] _hypeAmountDisplay;
    public float maxHype; //Essentially a win condition
    public Text winnerText;
    private void Awake()
    {
        if(HM == null) HM = this;
        else
        {
            HM._vehicleList.Clear();
            HM.SetUpDisplay();
        }
    }

    private void SetUpDisplay()
    {
        HM._hypeAmountDisplay = new Text[HM._vehicleList.Count];
        for (int i = 0; i < HM._hypeAmountDisplay.Length; i++)
        {
            HM._hypeAmountDisplay[i] = GameObject.Find("HypeDisplay" + (i + 1)).GetComponent<Text>();
        }

        //HM.BeginningVehicleSort();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Called to add the given vehicle to the vehicle list
    public void VehicleAssign(GameObject player)
    {
        _vehicleList.Add(player);
    }

    // Sorts the list based upon which game object has the most hype in their VehicleHypeBehavior script
    public void VehicleSort()
    {
        _vehicleList.Sort(
            delegate(GameObject p1, GameObject p2)
            {
                return p1.GetComponent<VehicleHypeBehavior>().GiveHypeAmount()
                .CompareTo(p2.GetComponent<VehicleHypeBehavior>().GiveHypeAmount());
            }
        );

        // Put in descending order
        _vehicleList.Reverse();
        checkWinCondition();
      //  UIupdate();
    }

    // Sorts the list at the beginning of the game based on player number rather than hype amount
    private void BeginningVehicleSort()
    {
        _vehicleList.Sort(
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
        foreach(GameObject entry in _vehicleList)
        {
            _hypeAmountDisplay[i].text = entry.name.ToString() + ": " +
           // _hypeAmountDisplay[i].text = "Hype: " +
            entry.GetComponent<VehicleHypeBehavior>().GiveHypeAmount().ToString();
            i++;
        }
    }

    private void checkWinCondition()
    {
        foreach(GameObject entry in _vehicleList)
        {
            if(entry.GetComponent<VehicleHypeBehavior>().GiveHypeAmount() >= maxHype)
            {
                Time.timeScale = 0.0f;
                winnerText.text = "PLAYER " + entry.GetComponent<VehicleInput>().playerNumber + " WINS!";
                winnerText.gameObject.SetActive(true);
            }
        }
    }

}

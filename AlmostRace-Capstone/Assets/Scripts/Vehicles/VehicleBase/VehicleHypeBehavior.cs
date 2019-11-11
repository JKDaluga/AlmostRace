using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

    /*
    Author: Jake Velicer
    Purpose: Stores the hype for individual car and reports
    newly gained hype to the HypeSystem script on the Game Manager.
    */

    /*
     Eddie B.
     Edit : 10/16/2019
     Because of a bug where the players' hype text locations are being flipped around based on who's in first, I'm going 
     to update the hype text locally for now.
     
     */
    
public class VehicleHypeBehavior : MonoBehaviour
{
    
    private HypeManager _hypeManagerScript;
    public float _hypeAmount;
    public TextMeshProUGUI hypeText;

    void Start()
    {
        if(HypeManager.instance != null)
        {
            _hypeManagerScript = HypeManager.instance;
            _hypeManagerScript.VehicleAssign(this.gameObject);
        }
    }

    private void Update()
    {
        UpdateUI();
    }

    public void UpdateUI()
    {
        hypeText.text = "Hype: " + _hypeAmount;
    }

    public void AddHype(float hypeToAdd)
    {
        _hypeAmount += hypeToAdd;
        _hypeManagerScript.VehicleSort();
    }

    public void SubtractHype(float hypeToSubtract)
    {
        _hypeAmount -= hypeToSubtract;
        _hypeManagerScript.VehicleSort();
    }

    public float GetHypeAmount()
    {
        return _hypeAmount;
    }

}

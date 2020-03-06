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
    public PlayerUIManager playerUIManagerScript;
    public float _hypeAmount;
    public float bigHypeAmount = 50f;

    void Start()
    {
        _hypeManagerScript = FindObjectOfType<HypeManager>();
        if (_hypeManagerScript != null)
        {
            _hypeManagerScript.VehicleAssign(this.gameObject);
        }
        else
        {
            Debug.LogWarning("Hype Manager not found!");
        }
    }

    public void AddHype(float hypeToAdd, string hypeType)
    {
        if(!_hypeManagerScript.isGameEnded)
        {
            if (hypeToAdd >= bigHypeAmount)
            {
                AudioManager.instance.PlayWithoutSpatial("Audience");
            }
            else
            {
                AudioManager.instance.PlayWithoutSpatial("Low Hype");
            }
            _hypeAmount = _hypeAmount + hypeToAdd;
        }
        else if (hypeType.Equals("Award"))
        {
            _hypeAmount = _hypeAmount + hypeToAdd;
        }
    }

    public void SubtractHype(float hypeToSubtract)
    {
        if (!_hypeManagerScript.isGameEnded)
        {
            _hypeAmount -= hypeToSubtract;
        }
    }

    public float GetHypeAmount()
    {
        return _hypeAmount;
    }

}

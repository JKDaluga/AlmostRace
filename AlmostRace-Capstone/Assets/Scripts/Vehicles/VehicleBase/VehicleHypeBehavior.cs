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
    public GameObject scalingPanel;
    public float _hypeAmount;
    public TextMeshProUGUI hypeText;
    public GameObject hypePopup;
    public Transform hypePopupSpawn;
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
            Debug.LogError("Hype Manager not found!");
        }
    }

    private void Update()
    {
        UpdateUI();
    }

    public void UpdateUI()
    {
        hypeText.text = "" + _hypeAmount;
    }

    public void AddHype(float hypeToAdd, string hypeType)
    {
       // Debug.Log(hypeToAdd + " was added to " + gameObject.name + " from " + hypeType);
       if(hypeToAdd >= bigHypeAmount)
        {
            AudioManager.instance.Play("Audience");
        }
        _hypeAmount += hypeToAdd;
        _hypeManagerScript.VehicleSort();
        GameObject spawnedPopUp = Instantiate(hypePopup, hypePopupSpawn.position, hypePopupSpawn.rotation);
        spawnedPopUp.GetComponent<TextMeshProUGUI>().text = hypeType + ": " + hypeToAdd;
        spawnedPopUp.transform.SetParent(scalingPanel.transform);
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

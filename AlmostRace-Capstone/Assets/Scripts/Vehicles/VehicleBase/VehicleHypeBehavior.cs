using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    /*
    Author: Jake Velicer
    Purpose: Stores the hype for individual car and reports
    newly gained hype to the HypeSystem script on the Game Manager.
    */
    
public class VehicleHypeBehavior : MonoBehaviour
{
    
    private HypeManager _hypeManagerScript;
    private float _hypeAmount;

    void Start()
    {
        if(HypeManager.HM != null)
        {
            _hypeManagerScript = HypeManager.HM;
            _hypeManagerScript.VehicleAssign(this.gameObject);
        }
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

    public float GiveHypeAmount()
    {
        return _hypeAmount;
    }

}

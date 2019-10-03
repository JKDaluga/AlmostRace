using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleHypeBehavior : MonoBehaviour
{
    /*
    Author: Jake Velicer
    Purpose: Stores the hype for individual car and reports
    newly gained hype to the HypeSystem script on the Game Manager.
    */
    
    private HypeSystem _hypeSystem;
    private float _hypeAmount;

    void Start()
    {
        if(GameObject.FindGameObjectWithTag("GameManager") != null)
        {
            _hypeSystem = GameObject.FindGameObjectWithTag("GameManager").GetComponent<HypeSystem>();
            _hypeSystem.VehicleAssign(this.gameObject);
        }
    }

    public void AddHype(float hypeToAdd)
    {
        _hypeAmount += hypeToAdd;
        _hypeSystem.VehicleSort();
    }

    public void SubtractHype(float hypeToSubtract)
    {
        _hypeAmount -= hypeToSubtract;
        _hypeSystem.VehicleSort();
    }

    public float GiveHypeAmount()
    {
        return _hypeAmount;
    }

}

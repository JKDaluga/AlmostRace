using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleHypeBehavior : MonoBehaviour
{
    public int playerNumber;
    private HypeSystem hypeSystem;
    private int _placeInRace;
    private int _positionInRace;

    // Start is called before the first frame update
    void Awake()
    {
        if(GameObject.FindGameObjectWithTag("GameManager") != null)
        {
            hypeSystem = GameObject.FindGameObjectWithTag("GameManager").GetComponent<HypeSystem>();
            hypeSystem.AssignVehicleSlot(this.gameObject, playerNumber);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

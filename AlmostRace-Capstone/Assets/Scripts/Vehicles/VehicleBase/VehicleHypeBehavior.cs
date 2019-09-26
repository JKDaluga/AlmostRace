using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleHypeBehavior : MonoBehaviour
{
    public float hypeAmount;
    [Tooltip("They are either player 1, 2, 3, or 4?")] public int playerNumber;
    private HypeSystem hypeSystem;
    private int _placeInRace;
    private int _positionInRace;

    // Start is called before the first frame update
    void Awake()
    {
        if(GameObject.FindGameObjectWithTag("GameManager") != null)
        {
            hypeSystem = GameObject.FindGameObjectWithTag("GameManager").GetComponent<HypeSystem>();
            //hypeSystem.AssignVehicleSlot(this.gameObject, playerNumber);
            hypeSystem.VehicleAssign(this.gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            AddHype(5);
        }
    }

    void AddHype(float hypeToAdd)
    {
        hypeSystem.AssignHype(gameObject, hypeToAdd);
    }

    void SubtractHype(float hypeToSubtract)
    {
        hypeSystem.AssignHype(gameObject, hypeToSubtract);
    }

}
